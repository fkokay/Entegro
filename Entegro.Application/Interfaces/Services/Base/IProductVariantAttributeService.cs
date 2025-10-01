using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductVariantAttribute;

namespace Entegro.Application.Interfaces.Services.Base
{
    public interface IProductVariantAttributeService
    {
        Task<ProductVariantAttributeDto?> GetByIdAsync(int id);
        Task<ProductVariantAttributeDto?> GetByAttibuteIdAsync(int productId, int attributeId);
        Task<List<ProductVariantAttributeDto>> GetAllAsync();
        Task<List<ProductVariantAttributeDto>> GetAllAsync(int productId);
        Task<PagedResult<ProductVariantAttributeDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7);
        Task<PagedResult<ProductVariantAttributeDto>> GetPagedAsync(GridCommand gridCommand, int productId);
        Task<ProductVariantAttributeDto> AddAsync(CreateProductVariantAttributeDto productAttributeMapping);
        Task<ProductVariantAttributeDto> UpdateAsync(UpdateProductVariantAttributeDto productAttributeMapping);
        Task DeleteAsync(int id);
    }
}
