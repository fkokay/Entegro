using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Log;

namespace Entegro.Application.Interfaces.Services
{
    public interface ILogService
    {
        Task<LogDto?> GetByIdAsync(int id);
        Task<List<LogDto>> GetAllLogsAsync();
        Task<PagedResult<LogDto>> GetLogsAsync(int page, string term);
        Task<PagedResult<LogDto>> GetPagedAsync(GridCommand gridCommand);
        Task<LogDto> AddAsync(CreateLogDto model);
        Task<LogDto> UpdateAsync(UpdateLogDto model);
        Task DeleteAsync(int id);
        Task DeleteAllAsync();
    }
}
