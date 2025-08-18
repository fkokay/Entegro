using Entegro.Domain.Entities;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface ICityRepository
    {
        Task<City> GetByIdAsync(int id);
        Task<List<City>> GetAllAsync();
        Task AddAsync(City city);
        Task UpdateAsync(City city);
        Task DeleteAsync(int id);
    }
}
