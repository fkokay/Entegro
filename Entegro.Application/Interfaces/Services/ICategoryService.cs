using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Common;

namespace Entegro.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<bool> ExistsByNameAsync(string name);
        Task<CategoryDto> GetCategoryByIdAsync(int categoryId);
        Task<CategoryDto> GetCategoryByNameAsync(string name);
        Task<IEnumerable<CategoryDto>> GetCategoriesAsync();
        Task<IEnumerable<CategoryTreePathDto>> GetCategoriesFormatTreePathAsync();
        Task<PagedResult<CategoryDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategory);
        Task<CategoryDto> UpdateCategoryAsync(UpdateCategoryDto updateCategory);
        Task DeleteCategoryAsync(int categoryId);
        Task<Select2ResponseDto> GetCategoriesForSelect2Async(string? term, int page, int pageSize);
        Task DeleteCategoryAndChildrenAsync(int categoryId);
        Task DeleteCategoryAndReassignChildrenAsync(int categoryId);
    }
}
