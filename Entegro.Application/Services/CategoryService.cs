using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Caching;
using Entegro.Collections;
using Entegro.Domain.Entities;
using MapsterMapper;
using System.Text;

namespace Entegro.Application.Services
{
    public class CategoryService : ICategoryService
    {
        internal static TimeSpan CategoryTreeCacheDuration = TimeSpan.FromHours(6);
        internal readonly static CompositeFormat CategoryTreeKey = CompositeFormat.Parse("category:tree-{0}");

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
            var category = _mapper.Map<Category>(updateCategory);
            await _categoryRepository.UpdateAsync(category);

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            var category = await _categoryRepository.GetByAsync(m => m.Id == categoryId);
            if (category == null)
                throw new KeyNotFoundException($"Category with ID {categoryId} not found.");

            await _categoryRepository.DeleteAsync(category);
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
            return new PagedResult<CategoryDto>
            {
                Items = _mapper.Map<IEnumerable<CategoryDto>>(categories.Items),
                TotalCount = categories.TotalCount,
                PageNumber = categories.PageNumber,
                PageSize = categories.PageSize
            };
        }

        public async Task<PagedResult<CategoryDto>> SearchPagedAsync(string? term, int page, int pageSize)
        {
            var categories = await _categoryRepository.GetAllAsync(term, page, pageSize);
            var items = _mapper.Map<IEnumerable<CategoryDto>>(categories.Items);


            return new PagedResult<CategoryDto>
            {
                Items = items,
                TotalCount = categories.TotalCount,
                PageNumber = categories.PageNumber,
                PageSize = categories.PageSize
            };
        }

        public async Task<string> GetCategoryPathAsync(ICategoryNode categoryNode, string separator = " » ")
        {
            var treeNode = await GetCategoryTreeAsync(categoryNode.Id, true);
            if (treeNode != null)
            {
                return GetCategoryPath(treeNode, separator);
            }

            return string.Empty;
        }

        public string GetCategoryPath(TreeNode<ICategoryNode> treeNode, string separator = " » ")
        {
            Guard.NotNull(treeNode);

            var lookupKey = "Path.{0}".FormatInvariant(separator);
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
