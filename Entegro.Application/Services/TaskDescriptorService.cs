using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.TaskDescriptor;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Services
{
    public class TaskDescriptorService : ITaskDescriptorService
    {
        private readonly ITaskDescriptorRepository _taskDescriptorRepository;
        private readonly IMapper _mapper;
        public TaskDescriptorService(ITaskDescriptorRepository taskDescriptorRepository,IMapper mapper)
        {
            _taskDescriptorRepository = taskDescriptorRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<TaskDescriptorDto>> GetPagedAsync(GridCommand gridCommand)
        {
            var taskDescriptors = await _taskDescriptorRepository.GetPagedAsync(gridCommand);

            var items = await taskDescriptors.Items.SelectAwait(async x =>
            {
                var model = _mapper.Map<TaskDescriptorDto>(x);
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
    }
}
