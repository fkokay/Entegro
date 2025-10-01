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

            var items = await info.Items.SelectAwait(async x =>
            {
                var model = _mapper.Map<TaskExecutionInfoDto>(x);
                model.StartedOn = x.StartedOnUtc.ToLocalTime();
                model.FinishedOn = x.FinishedOnUtc.ToLocalTime();
                model.SucceededOn = x.SucceededOnUtc.ToLocalTime();
                return model;
            }).AsyncToList();
            return new PagedResult<TaskExecutionInfoDto>
            {
                Items = items,
                TotalCount = info.TotalCount,
                PageNumber = info.PageNumber,
                PageSize = info.PageSize
            };
        }
    }
}
