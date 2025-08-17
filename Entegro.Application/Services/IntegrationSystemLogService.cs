using AutoMapper;
using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.IntegrationSystemLog;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;

namespace Entegro.Application.Services
{
    public class IntegrationSystemLogService : IIntegrationSystemLogService
    {
        private readonly IIntegrationSystemLogRepository _integrationSystemLogRepository;
        private readonly IMapper _mapper;

        public IntegrationSystemLogService(IIntegrationSystemLogRepository integrationSystemLog, IMapper mapper)
        {
            _integrationSystemLogRepository = integrationSystemLog ?? throw new ArgumentNullException(nameof(integrationSystemLog));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<int> AddAsync(CreateIntegrationSystemLogDto integrationSystemLog)
        {
            var model = _mapper.Map<IntegrationSystemLog>(integrationSystemLog);
            await _integrationSystemLogRepository.AddAsync(model);

            return model.Id;
        }

        public async Task<bool> DeleteAsync(int integrationSystemLogId)
        {
            var model = await _integrationSystemLogRepository.GetByIdAsync(integrationSystemLogId);

            if (model == null)
            {
                throw new KeyNotFoundException($"ProductAttribute with ID {integrationSystemLogId} not found.");
            }
            await _integrationSystemLogRepository.DeleteAsync(model);
            return true;
        }

        public async Task<List<IntegrationSystemLogDto>> GetAllAsync()
        {
            var integrationSystemLog = await _integrationSystemLogRepository.GetAllAsync();
            var IntegrationSystemLogDto = _mapper.Map<IEnumerable<IntegrationSystemLogDto>>(integrationSystemLog);
            return IntegrationSystemLogDto.ToList();
        }

        public async Task<PagedResult<IntegrationSystemLogDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var integrationSystemLogs = await _integrationSystemLogRepository.GetAllAsync(pageNumber, pageSize);
            var integrationSystemLogDto = _mapper.Map<PagedResult<IntegrationSystemLogDto>>(integrationSystemLogs);
            return integrationSystemLogDto;
        }

        public async Task<IntegrationSystemLogDto?> GetByIdAsync(int id)
        {
            var integrationSystemLog = await _integrationSystemLogRepository.GetByIdAsync(id);
            if (integrationSystemLog == null)
            {
                throw new KeyNotFoundException($"ProductAttribute with ID {id} not found.");
            }

            var integrationSystemLogDto = _mapper.Map<IntegrationSystemLogDto>(integrationSystemLog);
            return integrationSystemLogDto;
        }

        public async Task<bool> UpdateAsync(UpdateIntegrationSystemLogDto integrationSystemLog)
        {
            await _integrationSystemLogRepository.UpdateAsync(_mapper.Map<IntegrationSystemLog>(integrationSystemLog));
            return true;
        }
    }
}
