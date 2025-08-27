using Entegro.Domain.Entities;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IProductVariantAttributeValueRepository
    {
        Task<ProductVariantAttributeValue?> GetByNameAsync(string name);
        Task AddAsync(ProductVariantAttributeValue data);
    }
}
