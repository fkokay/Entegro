using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities.Catalog;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IProductAttributeRepository
    {
        Task<ProductAttribute?> GetByIdAsync(int id);
        Task<ProductAttribute?> GetByNameAsync(string name);
        Task<List<ProductAttribute>> GetAllAsync();
        Task<PagedResult<ProductAttribute>> GetAllAsync(int pageNumber, int pageSize);
        Task<PagedResult<ProductAttribute>> GetAllAsync(int page, string term);
        Task<PagedResult<ProductAttribute>> GetPagedAsync(GridCommand gridCommand);
        Task AddAsync(ProductAttribute productAttribute);
        Task UpdateAsync(ProductAttribute productAttribute);
        Task DeleteAsync(ProductAttribute productAttribute);
    }
}
