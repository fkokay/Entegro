using Entegro.Application.DTOs.Brand;
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

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategory)
        {
            var category = _mapper.Map<Category>(createCategory);
            await _categoryRepository.AddAsync(category);

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
            return new PagedResult<CategoryDto>
            {
                Items = _mapper.Map<IEnumerable<CategoryDto>>(categories.Items),
                TotalCount = categories.TotalCount,
                PageNumber = categories.PageNumber,
                PageSize = categories.PageSize
            };
        }

        public async Task<CategoryDto> UpdateCategoryAsync(UpdateCategoryDto updateCategory)
        {
            var category = _mapper.Map<Category>(updateCategory);
            await _categoryRepository.UpdateAsync(category);

            return _mapper.Map<CategoryDto>(category);
        }
    }
}
