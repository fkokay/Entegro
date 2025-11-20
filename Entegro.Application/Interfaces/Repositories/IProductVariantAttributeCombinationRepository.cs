using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities.Catalog;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IProductVariantAttributeCombinationRepository
    {
        Task<bool> ExistsAsync(int productId, string gtin);
        Task<ProductVariantAttributeCombination?> GetByIdAsync(int id);
        Task<List<ProductVariantAttributeCombination>> GetByProductIdAsync(int productId);
        Task<List<ProductVariantAttributeCombination>> GetAllAsync();
        Task<PagedResult<ProductVariantAttributeCombination>> GetAllAsync(int pageNumber, int pageSize);
        Task AddAsync(ProductVariantAttributeCombination productVariantAttributeCombination);
        Task UpdateAsync(ProductVariantAttributeCombination productVariantAttributeCombination);
        Task DeleteAsync(ProductVariantAttributeCombination productVariantAttributeCombination);
        Task DeleteByProductIdAsync(int productId);
    }
}
