using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.TaskDescriptor;

namespace Entegro.Application.Interfaces.Services
{
    public interface ITaskDescriptorService
    {
        Task<TaskDescriptorDto?> GetByIdAsync(int id);
        Task<TaskDescriptorDto> UpdateAsync(UpdateTaskDescriptorDto taskDescriptor);
        Task<PagedResult<TaskDescriptorDto>> GetPagedAsync(GridCommand gridCommand);
    }
}
