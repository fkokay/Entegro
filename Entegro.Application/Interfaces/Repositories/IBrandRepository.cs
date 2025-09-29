using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities.Catalog;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IBrandRepository
    {
        Task<bool> ExistsByIdAsync(int id);
        Task<bool> ExistsByNameAsync(string name);
        Task<Brand?> GetByIdAsync(int id);
        Task<Brand?> GetByNameAsync(string name);
        Task<List<Brand>> GetAllAsync();
        Task<PagedResult<Brand>> GetAllAsync(int page, string term);
        Task<PagedResult<Brand>> GetPagedAsync(GridCommand gridCommand);
        Task AddAsync(Brand brand);
        Task UpdateAsync(Brand brand);
        Task DeleteAsync(Brand brand);
        Task<Brand?> GetByIdWithMediaAsync(int id);
    }
}
