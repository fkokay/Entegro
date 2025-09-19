using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Caching;
using Entegro.Collections;
using Entegro.Domain.Entities.Catalog;
using MapsterMapper;
using System.Text;

namespace Entegro.Application.Services
{
    public class CategoryService : ICategoryService
    {
        // {0} = IncludeHidden,
        public static TimeSpan CategoryTreeCacheDuration = TimeSpan.FromHours(6);
        public readonly static CompositeFormat CategoryTreeKey = CompositeFormat.Parse("category:tree-{0}");
        public const string CategoryTreePatternKey = "category:tree-*";

        // {0} = IncludeHidden, {1} = ParentCategoryId
        internal readonly static CompositeFormat CategoriesByParentIdKey = CompositeFormat.Parse("category:byparent-{0}-{1}");
        public const string CategoriesPatternKey = "category:*";

        private readonly ICategoryRepository _categoryRepository;
        private readonly ICacheManager _cache;
        private readonly IMapper _mapper;
        public CategoryService(ICategoryRepository categoryRepository, ICacheManager cahce, IMapper mapper)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _cache = cahce ?? throw new ArgumentNullException(nameof(cahce));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategory)
        {
            var category = _mapper.Map<Category>(createCategory);
            await _categoryRepository.AddAsync(category);

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> UpdateCategoryAsync(UpdateCategoryDto updateCategory)
        {
            var category = await _categoryRepository.GetByAsync(m => m.Id == updateCategory.Id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {updateCategory.Id} not found.");
            }

            _mapper.Map<UpdateCategoryDto, Category>(updateCategory, category);
            await _categoryRepository.UpdateAsync(category);

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task DeleteCategoryAsync(int categoryId, bool deleteSubCategories = false)
        {
            var category = await _categoryRepository.GetByAsync(m => m.Id == categoryId);

            if (category == null)
                throw new KeyNotFoundException($"Category with ID {categoryId} not found.");

            category.Deleted = true;

            var subCategoryIds = await GetSubCategoryIds(new[] { category.Id });
            await SoftDeleteCategories(subCategoryIds);

            await _categoryRepository.UpdateAsync(category);

            async Task<IEnumerable<int>> GetSubCategoryIds(IEnumerable<int> categoryIds)
            {
                var result = new HashSet<int>();
                var ids = categoryIds.Distinct().ToArray();

                foreach (var id in ids)
                {
                    var tree = await GetCategoryTreeAsync(id, false);
                    if (tree?.HasChildren ?? false)
                    {
                        result.AddRange(tree.Children.Select(x => x.Value.Id));
                    }
                }

                return result;
            }

            async Task SoftDeleteCategories(IEnumerable<int> categoryIds)
            {
                if (categoryIds.Any())
                {
                    var categories = await _categoryRepository.GetManyAsync(categoryIds, true);

                    foreach (var category in categories)
                    {
                        if (deleteSubCategories)
                        {
                            category.Deleted = true;
                        }
                        else
                        {
                            category.ParentId = null;
                        }

                        var ids = await GetSubCategoryIds(categoryIds);
                        await SoftDeleteCategories(ids);

                        await _categoryRepository.UpdateAsync(category);
                    }
                }
            }
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _categoryRepository.ExistsAsync(m => m.Name == name);
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _categoryRepository.GetByAsync(m => m.Id == categoryId);
            return category == null ? null : _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto?> GetCategoryByNameAsync(string name)
        {
            var category = await _categoryRepository.GetByAsync(m => m.Name == name);
            return category == null ? null : _mapper.Map<CategoryDto>(category);
        }

        public async Task<PagedResult<CategoryDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7)
        {
            var categories = await _categoryRepository.GetAllAsync("", pageNumber, pageSize);
            var items = await categories.Items.SelectAwait(async x =>
            {
                var model = _mapper.Map<CategoryDto>(x);
                model.Breadcrumb = await GetCategoryPathAsync(x, "<small class='text-muted d-none d-sm-block'>{0}</small>");
                return model;
            }).AsyncToList();

            return new PagedResult<CategoryDto>
            {
                Items = items,
                TotalCount = categories.TotalCount,
                PageNumber = categories.PageNumber,
                PageSize = categories.PageSize
            };
        }

        public Task<PagedResult<CategoryDto>> GetPagedAsync(GridCommand gridCommand)
        {
            throw new NotImplementedException();
        }


        public async Task<string> GetCategoryPathAsync(ICategoryNode categoryNode, string aliasPattern = null, string separator = " » ")
        {
            var treeNode = await GetCategoryTreeAsync(categoryNode.Id, false);
            if (treeNode != null)
            {
                return GetCategoryPath(treeNode, aliasPattern, separator);
            }

            return categoryNode.Name;
        }

        public string GetCategoryPath(TreeNode<ICategoryNode> treeNode, string aliasPattern = null, string separator = " » ")
        {
            Guard.NotNull(treeNode);

            var lookupKey = "Path.{0}.{1}".FormatInvariant(separator, aliasPattern.HasValue());
            var cachedPath = treeNode.GetMetadata<string>(lookupKey, false);

            if (cachedPath != null)
            {
                return cachedPath;
            }

            var trail = treeNode.Trail;
            var sb = new StringBuilder(200);

            foreach (var node in trail)
            {
                if (!node.IsRoot)
                {
                    var cat = node.Value;

                    var name = cat.Name;

                    sb.Append(name);

                    if (node != treeNode)
                    {
                        sb.Append(separator);
                    }
                }
            }

            var path = sb.ToString();
            treeNode.SetContextMetadata(lookupKey, path);

            return path;
        }

        public async Task<TreeNode<ICategoryNode>> GetCategoryTreeAsync(int rootCategoryId = 0, bool includeHidden = false)
        {
            var cacheKey = CategoryTreeKey.FormatInvariant(includeHidden.ToString().ToLower());

            var root = await _cache.GetAsync(cacheKey, async o =>
            {
                o.ExpiresIn(CategoryTreeCacheDuration);

                var categories = await _categoryRepository.GetAllAsync(includeHidden);
                var unsortedNodes = categories
                    .Select(x => new CategoryNode
                    {
                        Id = x.Id,
                        ParentId = x.ParentId,
                        Name = x.Name,
                        MediaFileId = x.MediaFileId,
                        Published = x.Published,
                        DisplayOrder = x.DisplayOrder,
                        UpdatedOnUtc = x.UpdatedOnUtc,
                    });

                var nodeMap = unsortedNodes.ToMultimap(x => x.ParentId.GetValueOrDefault(), x => x);
                var curParent = new TreeNode<ICategoryNode>(new CategoryNode { Name = "Home" });

                AddChildTreeNodes(curParent, 0, nodeMap);

                return curParent.Root;
            });

            if (rootCategoryId > 0)
            {
                root = root.SelectNodeById(rootCategoryId);
            }

            return root;
        }

        private static void AddChildTreeNodes(TreeNode<ICategoryNode> parentNode, int parentItemId, Multimap<int, CategoryNode> nodeMap)
        {
            if (parentNode == null)
            {
                return;
            }

            var nodes = nodeMap.ContainsKey(parentItemId)
                ? nodeMap[parentItemId].OrderBy(x => x.DisplayOrder)
                : Enumerable.Empty<CategoryNode>();

            foreach (var node in nodes)
            {
                var newNode = new TreeNode<ICategoryNode>(node);
                parentNode.Append(newNode);
                AddChildTreeNodes(newNode, node.Id, nodeMap);
            }
        }
    }
}
