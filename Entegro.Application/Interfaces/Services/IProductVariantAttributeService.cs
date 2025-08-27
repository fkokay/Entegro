using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductVariantAttribute;

namespace Entegro.Application.Interfaces.Services
{
    public interface IProductVariantAttributeService
    {
        Task<ProductVariantAttributeDto?> GetByIdAsync(int id);
        Task<ProductVariantAttributeDto?> GetByAttibuteIdAsync(int productId, int attributeId);
        Task<List<ProductVariantAttributeDto>> GetAllAsync();
        Task<PagedResult<ProductVariantAttributeDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7);
        Task<ProductVariantAttributeDto> AddAsync(CreateProductVariantAttributeDto productAttributeMapping);
        Task<ProductVariantAttributeDto> UpdateAsync(UpdateProductVariantAttributeDto productAttributeMapping);
        Task DeleteAsync(int productAttributeMappingId);
    }
}
