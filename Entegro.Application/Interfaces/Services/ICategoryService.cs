using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Common;
using Entegro.Collections;
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
        Task<string> GetCategoryPathAsync(ICategoryNode categoryNode, string separator = " » ");
        string GetCategoryPath(TreeNode<ICategoryNode> treeNode, string separator = " » ");
        Task<TreeNode<ICategoryNode>> GetCategoryTreeAsync(int rootCategoryId = 0, bool includeHidden = false);
    }
}
