using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Setting;

namespace Entegro.Application.Interfaces.Services
{
    public interface ISettingService
    {
        Task<bool> ExistsByIdAsync(int id);
        Task<SettingDto?> GetByIdAsync(int id);
        Task<bool> ExistsByKeyAsync(string key);
        Task<SettingDto?> GetByKeyAsync(string key);

        Task<IEnumerable<SettingDto>> GetAllAsync();
        Task<PagedResult<SettingDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 7);
        Task<PagedResult<SettingDto>> GetPagedAsync(GridCommand gridCommand);
        Task<SettingDto> CreateAsync(CreateSettingDto model);
        Task<SettingDto> CreateAsync(string key, string value);
        Task<SettingDto> UpdateAsync(UpdateSettingDto model);
        Task<SettingDto> UpdateAsync(string key,string value);
        Task DeleteAsync(int id);

    }
}
