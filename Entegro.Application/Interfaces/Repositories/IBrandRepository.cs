using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IBrandRepository
    {
        Task<Brand?> GetByIdAsync(int id);
        Task<Brand?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name);
        Task<List<Brand>> GetAllAsync();
        Task<PagedResult<Brand>> GetAllAsync(int pageNumber, int pageSize);
        Task AddAsync(Brand brand);
        Task UpdateAsync(Brand brand);
        Task DeleteAsync(Brand brand);
        Task<Brand?> GetByIdWithMediaAsync(int id);
    }
}
