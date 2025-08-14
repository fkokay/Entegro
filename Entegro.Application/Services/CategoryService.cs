using AutoMapper;
using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;

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
        public async Task<int> CreateCategoryAsync(CreateCategoryDto createCategory)
        {

            var category = _mapper.Map<Category>(createCategory);

            // Önce ekle, böylece category.Id oluşur
            await _categoryRepository.AddAsync(category); // ID burada artık hazır

            // TreePath hesaplama
            if (category.ParentCategoryId == 0)
            {
                category.TreePath = $"/{category.Id}/";
            }
            else
            {
                var parentCategory = await _categoryRepository.GetByIdAsync(category.ParentCategoryId);
                if (parentCategory != null)
                {
                    category.TreePath = $"{parentCategory.TreePath}{category.Id}/";
                }
                else
                {
                    category.TreePath = $"/{category.Id}/"; // Ebeveyn bulunamazsa yine kök gibi
                }
            }

            // Güncelle ve kaydet
            await _categoryRepository.UpdateAsync(category); // TreePath güncelleniyor

            return category.Id;
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            var brand = await _categoryRepository.GetByIdAsync(categoryId);

            if (brand == null)
            {
                throw new KeyNotFoundException($"Category with ID {categoryId} not found.");
            }
            await _categoryRepository.DeleteAsync(brand);
            return true;
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            return categoryDtos;
        }

        public async Task<PagedResult<CategoryDto>> GetCategoriesAsync(int pageNumber, int pageSize)
        {
            var categories = await _categoryRepository.GetAllAsync(pageNumber, pageSize);
            var categoryDtos = _mapper.Map<PagedResult<CategoryDto>>(categories);
            return categoryDtos;
        }

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

        public async Task<bool> UpdateCategoryAsync(UpdateCategoryDto updateCategory)
        {
            await _categoryRepository.UpdateAsync(_mapper.Map<Category>(updateCategory));
            return true;
        }
    }
}
