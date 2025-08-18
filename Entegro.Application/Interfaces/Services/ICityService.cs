using Entegro.Application.DTOs.City;

namespace Entegro.Application.Interfaces.Services
{
    public interface ICityService
    {
        Task<List<CityDto>> GetAllAsync();
        Task<CityDto> GetByIdAsync(int id);
        Task AddAsync(CreateCityDto dto);
        Task UpdateAsync(UpdateCityDto dto);
        Task DeleteAsync(int id);
    }
}
