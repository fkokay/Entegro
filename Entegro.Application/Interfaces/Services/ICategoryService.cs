using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Common;

namespace Entegro.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<CategoryDto> GetCategoryByIdAsync(int categoryId);
        Task<IEnumerable<CategoryDto>> GetCategoriesAsync();
        Task<IEnumerable<CategoryTreePathDto>> GetCategoriesFormatTreePathAsync();
        Task<PagedResult<CategoryDto>> GetCategoriesAsync(int pageNumber, int pageSize);
        Task<int> CreateCategoryAsync(CreateCategoryDto createCategory);
        Task<bool> UpdateCategoryAsync(UpdateCategoryDto updateCategory);
        Task<bool> DeleteCategoryAsync(int categoryId);
    }
}
