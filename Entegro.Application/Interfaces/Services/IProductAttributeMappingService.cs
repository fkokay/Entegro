using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductAttributeMapping;

namespace Entegro.Application.Interfaces.Services
{
    public interface IProductAttributeMappingService
    {
        Task<ProductAttributeMappingDto?> GetByIdAsync(int id);
        Task<List<ProductAttributeMappingDto>> GetAllAsync();
        Task<PagedResult<ProductAttributeMappingDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<int> AddAsync(ProductAttributeMappingDto productAttributeMapping);
        Task<bool> UpdateAsync(ProductAttributeMappingDto productAttributeMapping);
        Task<bool> DeleteAsync(int productAttributeMappingId);
    }
}
