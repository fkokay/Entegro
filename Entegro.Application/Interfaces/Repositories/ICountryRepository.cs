using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface ICountryRepository
    {
        Task<PagedResult<Country>> GetAllAsync(int pageNumber, int pageSize);
        Task<Country> GetByIdAsync(int id);
        Task<List<Country>> GetAllAsync();
        Task AddAsync(Country country);
        Task UpdateAsync(Country country);
        Task DeleteAsync(int id);
    }
}
