using Entegro.Application.DTOs.Common;
using Entegro.Scheduling;

namespace Entegro.Application.Interfaces.Repositories
{
    public interface ITaskDescriptorRepository
    {

        Task<bool> ExistsByTypeAsync(string type);
        Task<TaskDescriptor?> GetByTypeAsync(string type);
        Task<TaskDescriptor?> GetByIdAsync(int id);
        Task UpdateAsync(TaskDescriptor taskDescriptor);
        Task<PagedResult<TaskDescriptor>> GetPagedAsync(GridCommand gridCommand);
    }
}
