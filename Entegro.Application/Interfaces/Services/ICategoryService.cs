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


        Task<bool> DeleteCategoryAndChildrenAsync(int categoryId);// kategori ve alt kategorileri sil
        Task<bool> DeleteCategoryAndReassignChildrenAsync(int categoryId); // kategori ve alt kategorileri sil, alt kategorileri başka bir kategoriye ata

        Task<Select2Response> GetCategoriesForSelect2Async(string? term, int page, int pageSize, CancellationToken ct = default);

        Task<CategoryDto?> GetByIdWithMediaAsync(int id);

        Task UpdateCategoryImageAsync(int categoryId, int mediaFileId);
        Task DeleteCategoryImageAsync(int categoryId);
    }
}
