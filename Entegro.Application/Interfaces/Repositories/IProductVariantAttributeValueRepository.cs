using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities.Catalog;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IProductVariantAttributeValueRepository
    {
        Task<ProductVariantAttributeValue?> GetByIdAsync(int id);
        Task<ProductVariantAttributeValue?> GetByProductVariantAttributeIdAsync(int productVariantAttributeId);
        Task<ProductVariantAttributeValue?> GetByNameAsync(int productVariantAttributeId, string name);
        Task<PagedResult<ProductVariantAttributeValue>> GetPagedAsync(GridCommand gridCommand, int productVariantAttributeId);
        Task AddAsync(ProductVariantAttributeValue data);
        Task UpdateAsync(ProductVariantAttributeValue data);
        Task DeleteAsync(ProductVariantAttributeValue data);
    }
}
