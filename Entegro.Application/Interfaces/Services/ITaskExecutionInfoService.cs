using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.TaskExecutionInfo;

namespace Entegro.Application.Interfaces.Services
{
    public interface ITaskExecutionInfoService
    {
        Task<TaskExecutionInfoDto?> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<PagedResult<TaskExecutionInfoDto>> GetPagedAsync(GridCommand gridCommand, int taskDescriptorId);
    }
}
