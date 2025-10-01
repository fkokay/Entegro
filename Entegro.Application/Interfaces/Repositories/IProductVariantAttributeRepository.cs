using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities.Catalog;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IProductVariantAttributeRepository
    {
        Task<ProductVariantAttribute?> GetByIdAsync(int id);
        Task<ProductVariantAttribute?> GetByAttributeIdAsync(int productId, int attributeId);
        Task<List<ProductVariantAttribute>> GetAllAsync();
        Task<List<ProductVariantAttribute>> GetAllAsync(int productId);
        Task<PagedResult<ProductVariantAttribute>> GetAllAsync(int pageNumber, int pageSize);
        Task<PagedResult<ProductVariantAttribute>> GetPagedAsync(GridCommand gridCommand, int productId);
        Task AddAsync(ProductVariantAttribute productAttributeMapping);
        Task UpdateAsync(ProductVariantAttribute productAttributeMapping);
        Task DeleteAsync(ProductVariantAttribute productAttributeMapping);
    }
}
