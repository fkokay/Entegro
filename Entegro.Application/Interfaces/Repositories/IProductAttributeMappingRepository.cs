using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IProductAttributeMappingRepository
    {
        Task<ProductVariantAttribute?> GetByIdAsync(int id);
        Task<ProductVariantAttribute?> GetByAttributeIdAsync(int id);
        Task<List<ProductVariantAttribute>> GetAllAsync();
        Task<PagedResult<ProductVariantAttribute>> GetAllAsync(int pageNumber, int pageSize);
        Task AddAsync(ProductVariantAttribute productAttributeMapping);
        Task UpdateAsync(ProductVariantAttribute productAttributeMapping);
        Task DeleteAsync(ProductVariantAttribute productAttributeMapping);
    }
}
