using Entegro.Application.DTOs.District;

namespace Entegro.Application.Interfaces.Services
{
    public interface IDistrictService
    {
        Task<List<DistrictDto>> GetAllAsync();
        Task<List<DistrictDto>> GetByTownIdAsync(int townId);
        Task<DistrictDto> GetByIdAsync(int id);
        Task AddAsync(CreateDistrictDto dto);
        Task UpdateAsync(UpdateDistrictDto dto);
        Task DeleteAsync(int id);
    }
}
