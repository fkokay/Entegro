using Entegro.Application.DTOs.Common;
using Entegro.Scheduling;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface ITaskExecutionInfoRepository
    {
        Task<TaskExecutionInfo?> GetByIdAsync(int id);
        Task DeleteAsync(TaskExecutionInfo info);
        Task<PagedResult<TaskExecutionInfo>> GetPagedAsync(GridCommand gridCommand, int taskDescriptorId);
    }
}
