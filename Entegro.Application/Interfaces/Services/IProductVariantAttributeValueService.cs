using Entegro.Application.DTOs.ProductVariantAttributeValue;

namespace Entegro.Application.Interfaces.Services
{
    public interface IProductVariantAttributeValueService
    {
        Task<ProductVariantAttributeValueDto?> GetByNameAsync(string name);
        Task<ProductVariantAttributeValueDto> AddAsync(CreateProductVariantAttributeValueDto data);
    }
}
