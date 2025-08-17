using Entegro.Domain.Entities;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IProductCategoryMappingRepository
    {
        Task<ProductCategoryMapping?> GetByIdAsync(int id);
        Task<List<ProductCategoryMapping>> GetAllAsync();
        Task AddAsync(ProductCategoryMapping productCategoryMapping);
        Task UpdateAsync(ProductCategoryMapping productCategoryMapping);
        Task DeleteAsync(ProductCategoryMapping productCategoryMapping);

        Task<List<ProductCategoryMapping>> GetByProductWithCategoryAsync(int productId, CancellationToken ct = default);
        Task<List<ProductCategoryMapping>> GetByProductsWithCategoryAsync(IEnumerable<int> productIds, CancellationToken ct = default);

    }
}
