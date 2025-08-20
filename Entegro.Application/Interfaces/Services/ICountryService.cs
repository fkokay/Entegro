using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Country;

namespace Entegro.Application.Interfaces.Services
{
    public interface ICountryService
    {
        Task<PagedResult<CountryDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<List<CountryDto>> GetAllAsync();
        Task<CountryDto> GetByIdAsync(int id);
        Task AddAsync(CreateCountryDto dto);
        Task UpdateAsync(UpdateCountryDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
