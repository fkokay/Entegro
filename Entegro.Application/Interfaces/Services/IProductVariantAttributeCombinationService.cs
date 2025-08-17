using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductVariantAttributeCombination;

namespace Entegro.Application.Interfaces.Services
{
    public interface IProductVariantAttributeCombinationService
    {
        Task<ProductVariantAttributeCombinationDto?> GetByIdAsync(int id);
        Task<List<ProductVariantAttributeCombinationDto>> GetAllAsync();
        Task<PagedResult<ProductVariantAttributeCombinationDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<int> AddAsync(ProductVariantAttributeCombinationDto productVariantAttributeCombinationDto);
        Task<bool> UpdateAsync(ProductVariantAttributeCombinationDto productVariantAttributeCombinationDto);
        Task<bool> DeleteAsync(int productVariantAttributeCombinationId);
    }
}
