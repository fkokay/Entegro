using Entegro.Application.DTOs.Town;

namespace Entegro.Application.Interfaces.Services
{
    public interface ITownService
    {
        Task<List<TownDto>> GetAllAsync();
        Task<List<TownDto>> GetByCityIdAsync(int cityId);
        Task<TownDto> GetByIdAsync(int id);
        Task AddAsync(CreateTownDto dto);
        Task UpdateAsync(UpdateTownDto dto);
        Task DeleteAsync(int id);
    }
}
