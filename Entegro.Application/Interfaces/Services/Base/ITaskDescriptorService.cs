using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.TaskDescriptor;

namespace Entegro.Application.Interfaces.Services.Base
{
    public interface ITaskDescriptorService
    {
        Task<bool> ExistsByTypeAsync(string type);
        Task<TaskDescriptorDto?> GetByTypeAsync(string type);
        Task<TaskDescriptorDto?> GetByIdAsync(int id);
        Task<TaskDescriptorDto> UpdateAsync(UpdateTaskDescriptorDto taskDescriptor);
        Task<PagedResult<TaskDescriptorDto>> GetPagedAsync(GridCommand gridCommand);
    }
}
