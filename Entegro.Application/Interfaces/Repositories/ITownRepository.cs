using Entegro.Domain.Entities;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface ITownRepository
    {
        Task<Town> GetByIdAsync(int id);
        Task<List<Town>> GetAllAsync();
        Task<List<Town>> GetByCityIdAsync(int cityId);
        Task AddAsync(Town town);
        Task UpdateAsync(Town town);
        Task DeleteAsync(int id);
    }
}
