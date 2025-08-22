using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductVariantAttribute;

namespace Entegro.Application.Interfaces.Services
{
    public interface IProductAttributeMappingService
    {
        Task<ProductVariantAttributeDto?> GetByIdAsync(int id);
        Task<ProductVariantAttributeDto?> GetByAttibuteIdAsync(int id);
        Task<List<ProductVariantAttributeDto>> GetAllAsync();
        Task<PagedResult<ProductVariantAttributeDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<int> AddAsync(CreateProductVariantAttributeDto productAttributeMapping);
        Task<bool> UpdateAsync(UpdateProductVariantAttributeDto productAttributeMapping);
        Task<bool> DeleteAsync(int productAttributeMappingId);
    }
}
