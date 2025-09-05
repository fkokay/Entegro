using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities;

namespace Entegro.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<bool> ExistsByNameAsync(string name);
        Task<CategoryDto?> GetCategoryByIdAsync(int categoryId);
        Task<CategoryDto?> GetCategoryByNameAsync(string name);
        Task<PagedResult<CategoryDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategory);
        Task<CategoryDto> UpdateCategoryAsync(UpdateCategoryDto updateCategory);
        Task DeleteCategoryAsync(int categoryId);
        Task<PagedResult<CategoryDto>> SearchPagedAsync(string? term, int page, int pageSize);
    }
}
