using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;
using MapsterMapper;

namespace Entegro.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // Yeni kategori oluşturma
        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategory)
        {

            var category = _mapper.Map<Category>(createCategory);


            await _categoryRepository.AddAsync(category);

            // TreePath hesaplama
            if (category.ParentCategoryId == null)
            {
                category.TreePath = $"/{category.Id}/";
            }
            else
            {
                var parentCategory = await _categoryRepository.GetByIdAsync(category.ParentCategoryId.Value);
                if (parentCategory != null)
                {
                    category.TreePath = $"{parentCategory.TreePath}{category.Id}/";
                }
                else
                {
                    category.TreePath = $"/{category.Id}/"; // Ebeveyn bulunamazsa yine kök gibi
                }
            }

            await _categoryRepository.UpdateAsync(category); // TreePath güncelleniyor
            return _mapper.Map<CategoryDto>(category);
        }

        // Kategori ve alt kategorilerini silme
        public async Task DeleteCategoryAndChildrenAsync(int categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
                throw new KeyNotFoundException($"Category with ID {categoryId} not found.");

            // Alt kategorileri getir
            var children = await _categoryRepository.GetByParentIdAsync(categoryId);

            // Alt kategorileri rekürsif olarak sil
            foreach (var child in children)
            {
                await DeleteCategoryAndChildrenAsync(child.Id);
            }

            // Kategoriyi sil
            await _categoryRepository.DeleteAsync(category);
        }

        // Kategori ve alt kategorilerini silme, alt kategorileri başka bir kategoriye atama
        public async Task DeleteCategoryAndReassignChildrenAsync(int categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
                throw new KeyNotFoundException($"Category with ID {categoryId} not found.");

            var children = await _categoryRepository.GetByParentIdAsync(categoryId);

            foreach (var child in children)
            {
                // Parent bağlantısını kopar
                child.ParentCategoryId = null;

                // TreePath güncelle
                child.TreePath = $"/{child.Id}/";
                await _categoryRepository.UpdateAsync(child);

                // Alt kategorilerin TreePath'lerini de güncelle
                await UpdateChildTreePathsRecursivelyAsync(child);
            }

            // Parent kategoriyi sil
            await _categoryRepository.DeleteAsync(category);
        }

        // Sadece kategori silme
        public async Task DeleteCategoryAsync(int categoryId)
        {
            Category? category = await _categoryRepository.GetByIdAsync(categoryId);

            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {categoryId} not found.");
            }
            await _categoryRepository.DeleteAsync(category);
        }


        // Kategori resmi silme
        public async Task DeleteCategoryImageAsync(int categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category != null)
            {
                category.MediaFileId = null;
                await _categoryRepository.UpdateAsync(category);
            }
        }


        // İsim ile kategori var mı kontrolü
        public async Task<bool> ExistsByNameAsync(string name) => await _categoryRepository.ExistsByNameAsync(name);


        // ID ile kategori ve medya dosyasını alma
        public async Task<CategoryDto?> GetByIdWithMediaAsync(int id)
        {
            var category = await _categoryRepository.GetByIdWithMediaAsync(id);
            if (category == null)
            {
                return null;
            }
            var categoryDto = _mapper.Map<CategoryDto>(category);
            return categoryDto;
        }


        // Tüm kategorileri alma
        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            return categoryDtos;
        }


        // Sayfalı kategori listeleme
        public async Task<PagedResult<CategoryDto>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var categories = await _categoryRepository.GetAllAsync(pageNumber, pageSize);
            var categoryDtos = _mapper.Map<PagedResult<CategoryDto>>(categories);
            return categoryDtos;
        }


        // Kategorileri TreePath formatında alma
        public async Task<IEnumerable<CategoryTreePathDto>> GetCategoriesFormatTreePathAsync()
        {
            var categories = await GetCategoriesAsync();

            // Kategorileri TreePath'e göre sıralıyoruz
            var orderedCategories = categories.OrderBy(c => c.TreePath).ToList();

            // Kategorilerdeki ID'ler üzerinden isimleri bulup, TreePath formatını düzeltiyoruz
            var categoryDtos = orderedCategories.Select(category => new CategoryDto
            {
                Id = category.Id,
                ParentCategoryId = category.ParentCategoryId,
                TreePath = category.TreePath,
                Name = category.Name,
                Description = category.Description,
                MetaTitle = category.MetaTitle,
                MetaDescription = category.MetaDescription,
                MetaKeywords = category.MetaKeywords,
                DisplayOrder = category.DisplayOrder,
                CreatedOn = category.CreatedOn,
                UpdatedOn = category.UpdatedOn
            }).ToList();

            // Burada, kategorileri TreePath'e göre formatlayacağız
            var result = categoryDtos.Select(c => new CategoryTreePathDto
            {
                Id = c.Id,
                Name = c.Name,
                FormattedName = FormatTreePath(c.TreePath, categoryDtos)
            }).ToList();

            return result;
        }


        // Select2 için kategori listeleme
        public async Task<Select2ResponseDto> GetCategoriesForSelect2Async(string? term, int page, int pageSize)
        {
            var paged = await _categoryRepository.SearchPagedAsync(term, page, pageSize);

            // Sayfadaki kayıtların tüm ata ID’leri
            var ancestorIds = paged.Items
                .SelectMany(r => (r.TreePath ?? "/").Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries))
                .Select(s => int.TryParse(s, out var i) ? i : 0)
                .Where(i => i > 0)
                .Distinct()
                .ToList();

            var names = await _categoryRepository.GetNamesByIdsAsync(ancestorIds);

            static string FormatTreePath(string? treePath, IReadOnlyDictionary<int, string> map, string fallback)
            {
                if (string.IsNullOrWhiteSpace(treePath)) return fallback;
                var parts = treePath.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
                var chain = new List<string>(parts.Length);
                foreach (var p in parts)
                {
                    if (int.TryParse(p, out var id) && map.TryGetValue(id, out var nm))
                        chain.Add(nm);
                }
                return chain.Count > 0 ? string.Join(" - ", chain) : fallback;
            }

            var result = new Select2ResponseDto
            {
                results = paged.Items.Select(r => new Select2OptionDto
                {
                    id = r.Id,
                    text = FormatTreePath(r.TreePath, names, r.Name)
                }).ToList(),
                pagination = new Select2ResponseDto.Pagination { more = paged.HasMore }
            };

            return result;
        }


        // ID ile kategori alma
        public async Task<CategoryDto> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {categoryId} not found.");
            }

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return categoryDto;
        }


        // İsim ile kategori alma
        public async Task<CategoryDto> GetCategoryByNameAsync(string name)
        {
            var category = await _categoryRepository.GetByNameAsync(name);
            if (category == null)
            {
                return null;
            }

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return categoryDto;
        }


        // Kategori güncelleme
        public async Task<CategoryDto> UpdateCategoryAsync(UpdateCategoryDto updateCategory)
        {
            var category = _mapper.Map<Category>(updateCategory);

            if (category.ParentCategoryId == null)
            {
                category.TreePath = $"/{category.Id}/";
            }
            else
            {
                var parentCategory = await _categoryRepository.GetByIdAsync(category.ParentCategoryId.Value);
                if (parentCategory != null)
                {
                    category.TreePath = $"{parentCategory.TreePath}{category.Id}/";
                }
                else
                {
                    category.TreePath = $"/{category.Id}/"; // Ebeveyn bulunamazsa kök gibi
                }
            }
            await _categoryRepository.UpdateAsync(category);
            return _mapper.Map<CategoryDto>(category);
        }


        // Kategori resmi güncelleme
        public async Task UpdateCategoryImageAsync(int categoryId, int mediaFileId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category != null)
            {
                category.MediaFileId = mediaFileId;
                await _categoryRepository.UpdateAsync(category);
            }
        }



        #region yardımcı metotlar
        private string FormatTreePath(string treePath, List<CategoryDto> allCategories)
        {
            // TreePath'i id'ler üzerinden çözümleyelim
            var pathIds = treePath.Trim('/').Split('/');
            var categoryNames = new List<string>();

            foreach (var pathId in pathIds)
            {
                // ID'ye karşılık gelen kategori ismini buluyoruz
                var category = allCategories.FirstOrDefault(c => c.Id.ToString() == pathId);
                if (category != null)
                {
                    categoryNames.Add(category.Name);
                }
            }

            // Kategori isimlerini " - " ile birleştiriyoruz
            return string.Join(" - ", categoryNames);
        }

        private async Task UpdateChildTreePathsRecursivelyAsync(Category parentCategory)
        {
            var childCategories = await _categoryRepository.GetByParentIdAsync(parentCategory.Id);

            foreach (var child in childCategories)
            {
                // Yeni TreePath: parent'ın TreePath'i + kendi ID'si
                child.TreePath = $"{parentCategory.TreePath}{child.Id}/";

                await _categoryRepository.UpdateAsync(child);

                // Alt kategoriler için kendini tekrar çağır
                await UpdateChildTreePathsRecursivelyAsync(child);
            }
        }
        #endregion


    }
}
