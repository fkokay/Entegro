using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductAttribute;

namespace Entegro.Application.Interfaces.Services
{
    public interface IProductAttributeService
    {
        Task<ProductAttributeDto?> GetByIdAsync(int id);
        Task<List<ProductAttributeDto>> GetAllAsync();
        Task<PagedResult<ProductAttributeDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<int> AddAsync(CreateProductAttributeDto productAttribute);
        Task<bool> UpdateAsync(UpdateProductAttributeDto productAttribute);
        Task<bool> DeleteAsync(int productAttributeId);
    }
}
