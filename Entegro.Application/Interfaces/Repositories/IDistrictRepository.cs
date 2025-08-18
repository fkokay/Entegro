using Entegro.Domain.Entities;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface IDistrictRepository
    {
        Task<District> GetByIdAsync(int id);
        Task<List<District>> GetAllAsync();
        Task<List<District>> GetByTownIdAsync(int townId);
        Task AddAsync(District district);
        Task UpdateAsync(District district);
        Task DeleteAsync(int id);
    }
}
