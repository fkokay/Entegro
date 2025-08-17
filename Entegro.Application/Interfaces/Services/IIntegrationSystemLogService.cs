using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.IntegrationSystemLog;

namespace Entegro.Application.Interfaces.Services
{
    public interface IIntegrationSystemLogService
    {
        Task<IntegrationSystemLogDto?> GetByIdAsync(int id);
        Task<List<IntegrationSystemLogDto>> GetAllAsync();
        Task<PagedResult<IntegrationSystemLogDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<int> AddAsync(CreateIntegrationSystemLogDto integrationSystemLog);
        Task<bool> UpdateAsync(UpdateIntegrationSystemLogDto integrationSystemLog);
        Task<bool> DeleteAsync(int integrationSystemLogId);
    }
}
