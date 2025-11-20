using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities.Catalog;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IProductCategoryRepository
    {
        Task<ProductCategory?> GetByIdAsync(int id);
        Task<List<ProductCategory>> GetAllAsync();
        Task DeleteByProductIdAsync(int productId);
        Task AddAsync(ProductCategory productCategoryMapping);
        Task UpdateAsync(ProductCategory productCategoryMapping);
        Task DeleteAsync(ProductCategory productCategoryMapping);
        Task<List<ProductCategory>> GetByProductWithCategoryAsync(int productId);
        Task<List<ProductCategory>> GetByProductsWithCategoryAsync(IEnumerable<int> productIds);
        Task<PagedResult<ProductCategory>> GetPagedAsync(GridCommand gridCommand, int productId);

    }
}
