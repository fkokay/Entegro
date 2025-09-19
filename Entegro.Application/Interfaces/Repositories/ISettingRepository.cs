using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities.Setttings;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface ISettingRepository
    {
        Task<bool> ExistsByIdAsync(int id);
        Task<bool> ExistsByValueAsync(string value);
        Task<bool> ExistsByKeyAsync(string key);
        Task<Setting?> GetByIdAsync(int id);
        Task<List<Setting>> GetAllAsync();
        Task<PagedResult<Setting>> GetAllAsync(int pageNumber, int pageSize);
        Task<PagedResult<Setting>> GetPagedAsync(GridCommand gridCommand);
        Task AddAsync(Setting model);
        Task UpdateAsync(Setting model);
        Task DeleteAsync(Setting model);
        Task<Setting?> GetByValueAsync(string value);
        Task<Setting?> GetByKeyAsync(string key);
    }
}

