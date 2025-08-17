using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IProductVariantAttributeCombinationRepository
    {
        Task<ProductVariantAttributeCombination?> GetByIdAsync(int id);
        Task<List<ProductVariantAttributeCombination>> GetAllAsync();
        Task<PagedResult<ProductVariantAttributeCombination>> GetAllAsync(int pageNumber, int pageSize);
        Task AddAsync(ProductVariantAttributeCombination productVariantAttributeCombination);
        Task UpdateAsync(ProductVariantAttributeCombination productVariantAttributeCombination);
        Task DeleteAsync(ProductVariantAttributeCombination productVariantAttributeCombination);
    }
}
