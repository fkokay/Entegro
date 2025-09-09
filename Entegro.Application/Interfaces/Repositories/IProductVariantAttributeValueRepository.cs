using Entegro.Domain.Entities.Catalog;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IProductVariantAttributeValueRepository
    {
        Task<ProductVariantAttributeValue?> GetByIdAsync(int id);
        Task<ProductVariantAttributeValue?> GetByNameAsync(int productVariantAttributeId, string name);
        Task AddAsync(ProductVariantAttributeValue data);
    }
}
