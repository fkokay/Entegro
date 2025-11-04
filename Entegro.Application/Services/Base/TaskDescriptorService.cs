using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.TaskDescriptor;
using Entegro.Application.DTOs.TaskExecutionInfo;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Scheduling;
using MapsterMapper;

namespace Entegro.Application.Services.Base
{
    public class TaskDescriptorService : ITaskDescriptorService
    {
        private readonly ITaskDescriptorRepository _taskDescriptorRepository;
        private readonly IMapper _mapper;
        public TaskDescriptorService(ITaskDescriptorRepository taskDescriptorRepository, IMapper mapper)
        {
            _taskDescriptorRepository = taskDescriptorRepository;
            _mapper = mapper;
        }

        public async Task<bool> ExistsByTypeAsync(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Görev tipi boş olamaz.", nameof(type));

            return await _taskDescriptorRepository.ExistsByTypeAsync(type);
        }

        public async Task<TaskDescriptorDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var taskDescriptor = await _taskDescriptorRepository.GetByIdAsync(id);

            if (taskDescriptor == null)
            {
                return null;
            }

            var taskDescriptorDto = _mapper.Map<TaskDescriptorDto>(taskDescriptor);
            taskDescriptorDto.CronDescription = CronExpression.GetFriendlyDescription(taskDescriptor.CronExpression);
            taskDescriptorDto.NextRun = taskDescriptor.NextRunUtc.ToLocalTime();
            taskDescriptorDto.LastExecution = taskDescriptor.ExecutionHistory.Any() ? _mapper.Map<TaskExecutionInfoDto>(taskDescriptor.ExecutionHistory.Last()) : null;
            if (taskDescriptor.LastExecution != null)
            {
                taskDescriptorDto.LastExecution.StartedOn = taskDescriptor.ExecutionHistory.Last().StartedOnUtc.ToLocalTime();
                taskDescriptorDto.LastExecution.FinishedOn = taskDescriptor.ExecutionHistory.Last().FinishedOnUtc.ToLocalTime();
                taskDescriptorDto.LastExecution.SucceededOn = taskDescriptor.ExecutionHistory.Last().SucceededOnUtc.ToLocalTime();
            }
            return taskDescriptorDto;
        }

        public async Task<TaskDescriptorDto?> GetByTypeAsync(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                throw new ArgumentException("Görev tipi boş olamaz.", nameof(type));
            }

            var task = await _taskDescriptorRepository.GetByTypeAsync(type);
            var taskDto = _mapper.Map<TaskDescriptorDto>(task);

            return taskDto;
        }

        public async Task<PagedResult<TaskDescriptorDto>> GetPagedAsync(GridCommand gridCommand)
        {
            var taskDescriptors = await _taskDescriptorRepository.GetPagedAsync(gridCommand);

            var items = await taskDescriptors.Items.SelectAwait(async x =>
            {
                var model = _mapper.Map<TaskDescriptorDto>(x);
                model.CronDescription = CronExpression.GetFriendlyDescription(x.CronExpression);
                model.NextRun = x.NextRunUtc.ToLocalTime();
                model.LastExecution = x.ExecutionHistory.Any() ? _mapper.Map<TaskExecutionInfoDto>(x.ExecutionHistory.Last()) : null;
                if (model.LastExecution != null)
                {
                    model.LastExecution.StartedOn = x.ExecutionHistory.Last().StartedOnUtc.ToLocalTime();
                    model.LastExecution.FinishedOn = x.ExecutionHistory.Last().FinishedOnUtc.ToLocalTime();
                    model.LastExecution.SucceededOn = x.ExecutionHistory.Last().SucceededOnUtc.ToLocalTime();
                }

                return model;
            }).AsyncToList();

            return new PagedResult<TaskDescriptorDto>
            {
                Items = items,
                TotalCount = taskDescriptors.TotalCount,
                PageNumber = taskDescriptors.PageNumber,
                PageSize = taskDescriptors.PageSize
            };
        }

        public async Task<TaskDescriptorDto> UpdateAsync(UpdateTaskDescriptorDto taskDescriptor)
        {
            if (taskDescriptor == null)
                throw new ArgumentNullException(nameof(taskDescriptor));

            var existingTaskDescriptor = await _taskDescriptorRepository.GetByIdAsync(taskDescriptor.Id);
            if (existingTaskDescriptor == null)
                throw new KeyNotFoundException($"ID {taskDescriptor.Id} ile TaskDescriptor bulunamadı.");

            _mapper.Map(taskDescriptor, existingTaskDescriptor);
            await _taskDescriptorRepository.UpdateAsync(existingTaskDescriptor);

            return _mapper.Map<TaskDescriptorDto>(existingTaskDescriptor);
        }
    }
}
