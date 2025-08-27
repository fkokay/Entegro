using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductAttribute;

namespace Entegro.Application.Interfaces.Services
{
    public interface IProductAttributeService
    {
        Task<ProductAttributeDto?> GetByIdAsync(int id);
        Task<ProductAttributeDto?> GetByNameAsync(string name);
        Task<List<ProductAttributeDto>> GetAllAsync();
        Task<PagedResult<ProductAttributeDto>> GetPagedAsync(int pageNumber, int pageSize);
        Task<ProductAttributeDto> AddAsync(CreateProductAttributeDto productAttribute);
        Task<ProductAttributeDto> UpdateAsync(UpdateProductAttributeDto productAttribute);
        Task DeleteAsync(int productAttributeId);
    }
}
