using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.TaskExecutionInfo;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using MapsterMapper;

namespace Entegro.Application.Services
{
    public class TaskExecutionInfoService : ITaskExecutionInfoService
    {
        private readonly ITaskExecutionInfoRepository _taskExecutionInfoRepository;
        private readonly IMapper _mapper;
        public TaskExecutionInfoService(ITaskExecutionInfoRepository taskExecutionInfoRepository, IMapper mapper)
        {
            _taskExecutionInfoRepository = taskExecutionInfoRepository ?? throw new ArgumentNullException(nameof(taskExecutionInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            var info = await _taskExecutionInfoRepository.GetByIdAsync(id);
            if (info == null)
                throw new KeyNotFoundException($"ID {id} ile Info bulunamadı.");

            await _taskExecutionInfoRepository.DeleteAsync(info);
            return true;
        }

        public async Task<TaskExecutionInfoDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var info = await _taskExecutionInfoRepository.GetByIdAsync(id);
            if (info == null)
            {
                return null;
            }
            var infoDto = _mapper.Map<TaskExecutionInfoDto>(info);

            return infoDto;
        }

        public async Task<PagedResult<TaskExecutionInfoDto>> GetPagedAsync(GridCommand gridCommand, int taskDescriptorId)
        {
            var info = await _taskExecutionInfoRepository.GetPagedAsync(gridCommand, taskDescriptorId);
            return new PagedResult<TaskExecutionInfoDto>
            {
                Items = _mapper.Map<IEnumerable<TaskExecutionInfoDto>>(info.Items),
                TotalCount = info.TotalCount,
                PageNumber = info.PageNumber,
                PageSize = info.PageSize
            };
        }
    }
}
