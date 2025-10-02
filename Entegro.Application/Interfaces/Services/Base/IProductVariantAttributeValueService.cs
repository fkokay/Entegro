using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductVariantAttributeValue;

namespace Entegro.Application.Interfaces.Services.Base
{
    public interface IProductVariantAttributeValueService
    {
        Task<ProductVariantAttributeValueDto?> GetByIdAsync(int id);
        Task<ProductVariantAttributeValueDto?> GetByProductVariantAttributeIdAsync(int productVariantAttributeId);
        Task<ProductVariantAttributeValueDto?> GetByNameAsync(int productVariantAttributeId, string name);
        Task<ProductVariantAttributeValueDto> AddAsync(CreateProductVariantAttributeValueDto data);
        Task<PagedResult<ProductVariantAttributeValueDto>> GetPagedAsync(GridCommand gridCommand, int productVariantAttributeId);
        Task<ProductVariantAttributeValueDto> UpdateAsync(UpdateProductVariantAttributeValueDto data);
        Task DeleteAsync(int id);
    }
}
