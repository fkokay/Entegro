using Entegro.Application.DTOs.Common;
using Entegro.Domain.Entities.Platform.Logging;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface ILogRepository
    {
        Task<Log?> GetByIdAsync(int id);
        Task<List<Log>> GetAllAsync();
        Task<PagedResult<Log>> GetAllAsync(int page, string term);
        Task<PagedResult<Log>> GetPagedAsync(GridCommand gridCommand);
        Task AddAsync(Log log);
        Task UpdateAsync(Log log);
        Task DeleteAsync(Log log);
        Task DeleteAllAsync();
    }
}
