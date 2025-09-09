using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Common;
using Entegro.Collections;
using Entegro.Domain.Entities.Catalog;

namespace Entegro.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<bool> ExistsByNameAsync(string name);
        Task<CategoryDto?> GetCategoryByIdAsync(int categoryId);
        Task<CategoryDto?> GetCategoryByNameAsync(string name);
        Task<PagedResult<CategoryDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7);
        Task<PagedResult<CategoryDto>> GetPagedAsync(GridCommand gridCommand);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategory);
        Task<CategoryDto> UpdateCategoryAsync(UpdateCategoryDto updateCategory);
        Task DeleteCategoryAsync(int categoryId, bool deleteSubCategories);
        Task<string> GetCategoryPathAsync(ICategoryNode categoryNode, string aliasPattern = null, string separator = " » ");
        string GetCategoryPath(TreeNode<ICategoryNode> treeNode, string aliasPattern = null, string separator = " » ");
        Task<TreeNode<ICategoryNode>> GetCategoryTreeAsync(int rootCategoryId = 0, bool includeHidden = false);
    }
}
