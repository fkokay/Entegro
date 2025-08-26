using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductAttributeValue;
using Entegro.Domain.Entities;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IProductAttributeValueRepository
    {
        Task<ProductAttributeValue?> GetByIdAsync(int id);
        Task<ProductAttributeValue?> GetByNameAsync(string name);
        Task<List<ProductAttributeValue>> GetAllAsync();
        Task<PagedResult<ProductAttributeValue>> GetAllAsync(int pageNumber, int pageSize);
        Task AddAsync(ProductAttributeValue productAttributeValue);
        Task UpdateAsync(ProductAttributeValue productAttributeValue);
        Task DeleteAsync(ProductAttributeValue productAttributeValue);
    }
}
