using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductVariantAttributeCombination;

namespace Entegro.Application.Interfaces.Services.Base
{
    public interface IProductVariantAttributeCombinationService
    {
        Task<bool> ExistsAsync(int productId, string gtin);
        Task<ProductVariantAttributeCombinationDto?> GetByIdAsync(int id);
        Task<List<ProductVariantAttributeCombinationDto>> GetByProductIdAsync(int productId);
        Task<List<ProductVariantAttributeCombinationDto>> GetAllAsync();
        Task<PagedResult<ProductVariantAttributeCombinationDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7);
        Task<ProductVariantAttributeCombinationDto> AddAsync(CreateProductVariantAttributeCombinationDto productVariantAttributeCombinationDto);
        Task<ProductVariantAttributeCombinationDto> UpdateAsync(UpdateProductVariantAttributeCombinationDto productVariantAttributeCombinationDto);
        Task DeleteAsync(int productVariantAttributeCombinationId);
        Task DeleteByProductIdAsync(int productId);
    }
}
