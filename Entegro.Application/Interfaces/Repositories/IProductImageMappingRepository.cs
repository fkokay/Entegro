using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IProductImageMappingRepository
    {
        Task<ProductImageMapping?> GetByIdAsync(int id);
        Task<List<ProductImageMapping>> GetAllAsync();
        Task AddAsync(ProductImageMapping productImage);
        Task UpdateAsync(ProductImageMapping productImage);
        Task DeleteAsync(ProductImageMapping productImage);
        Task<PagedResult<ProductImageMapping>> GetAllAsync(int pageNumber, int pageSize);
    }
}
