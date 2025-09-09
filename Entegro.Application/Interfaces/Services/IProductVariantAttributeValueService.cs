using Entegro.Application.DTOs.ProductVariantAttributeValue;

namespace Entegro.Application.Interfaces.Services
{
    public interface IProductVariantAttributeValueService
    {
        Task<ProductVariantAttributeValueDto?> GetByIdAsync(int id);
        Task<ProductVariantAttributeValueDto?> GetByNameAsync(int productVariantAttributeId,string name);
        Task<ProductVariantAttributeValueDto> AddAsync(CreateProductVariantAttributeValueDto data);
    }
}
