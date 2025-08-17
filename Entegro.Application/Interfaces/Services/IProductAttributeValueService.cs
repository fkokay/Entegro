using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductAttributeValue;

namespace Entegro.Application.Interfaces.Services
{
    public interface IProductAttributeValueService
    {
        Task<ProductAttributeValueDto?> GetByIdAsync(int id);
        Task<List<ProductAttributeValueDto>> GetAllAsync();
        Task<PagedResult<ProductAttributeValueDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<int> AddAsync(CreateProductAttributeValueDto productAttributeValue);
        Task<bool> UpdateAsync(UpdateProductAttributeValueDto productAttributeValue);
        Task<bool> DeleteAsync(int productAttributeId);
    }
}
