using Entegro.Application.DTOs.ProductSpecificationAttribute;

namespace Entegro.Application.Interfaces.Services.Base
{
    public interface IProductSpecificationAttributeMappingService
    {
        Task<ProductSpecificationAttributeDto?> GetByIdAsync(int id);
        Task<List<ProductSpecificationAttributeDto>> GetAllAsync();
        Task<ProductSpecificationAttributeDto> AddAsync(CreateProductSpecificationAttributeDto productSpecificationAttribute);
        Task<ProductSpecificationAttributeDto> UpdateAsync(UpdateProductSpecificationAttributeDto productSpecificationAttribute);
        Task DeleteAsync(int id);
        Task<List<ProductSpecificationAttributeDto>> GetSpecificationAttributeByProductId(int productId);
    }
}
