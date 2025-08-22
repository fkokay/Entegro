using Entegro.Domain.Entities;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IProductCategoryMappingRepository
    {
        Task<ProductCategory?> GetByIdAsync(int id);
        Task<List<ProductCategory>> GetAllAsync();
        Task AddAsync(ProductCategory productCategoryMapping);
        Task UpdateAsync(ProductCategory productCategoryMapping);
        Task DeleteAsync(ProductCategory productCategoryMapping);

        Task<List<ProductCategory>> GetByProductWithCategoryAsync(int productId, CancellationToken ct = default);
        Task<List<ProductCategory>> GetByProductsWithCategoryAsync(IEnumerable<int> productIds, CancellationToken ct = default);

    }
}
