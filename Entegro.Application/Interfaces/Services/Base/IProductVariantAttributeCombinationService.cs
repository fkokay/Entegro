using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductVariantAttributeCombination;

namespace Entegro.Application.Interfaces.Services.Base
{
    public interface IProductVariantAttributeCombinationService
    {
        Task<ProductVariantAttributeCombinationDto?> GetByIdAsync(int id);
        Task<List<ProductVariantAttributeCombinationDto>> GetAllAsync();
        Task<PagedResult<ProductVariantAttributeCombinationDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7);
        Task<ProductVariantAttributeCombinationDto> AddAsync(CreateProductVariantAttributeCombinationDto productVariantAttributeCombinationDto);
        Task<ProductVariantAttributeCombinationDto> UpdateAsync(UpdateProductVariantAttributeCombinationDto productVariantAttributeCombinationDto);
        Task DeleteAsync(int productVariantAttributeCombinationId);
    }
}
