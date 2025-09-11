using Entegro.Domain.Entities.Catalog;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IProductSpecificationAttributeMappingRepository
    {
        Task<ProductSpecificationAttribute?> GetByIdAsync(int id);
        Task<List<ProductSpecificationAttribute>> GetAllAsync();
        Task AddAsync(ProductSpecificationAttribute productSpecificationAttribute);
        Task UpdateAsync(ProductSpecificationAttribute productSpecificationAttribute);
        Task DeleteAsync(ProductSpecificationAttribute productSpecificationAttribute);
        Task<List<ProductSpecificationAttribute>> GetSpecificationAttributeByProductId(int productId);
    }
}
