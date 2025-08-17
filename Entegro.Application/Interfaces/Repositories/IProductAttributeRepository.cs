using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IProductAttributeRepository
    {
        Task<ProductAttribute?> GetByIdAsync(int id);
        Task<List<ProductAttribute>> GetAllAsync();
        Task<PagedResult<ProductAttribute>> GetAllAsync(int pageNumber, int pageSize);
        Task AddAsync(ProductAttribute productAttribute);
        Task UpdateAsync(ProductAttribute productAttribute);
        Task DeleteAsync(ProductAttribute productAttribute);
    }
}
