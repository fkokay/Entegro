using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Log;
using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services.Base;
using MapsterMapper;

namespace Entegro.Application.Services.Base
{
    public class LogService : ILogService
    {
        private readonly ILogRepository _logRepository;
        private readonly IMapper _mapper;
        public LogService(ILogRepository logRepository, IMapper mapper)
        {
            _logRepository = logRepository ?? throw new ArgumentNullException(nameof(logRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public Task<LogDto> AddAsync(CreateLogDto model)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAllAsync()
        {
            await _logRepository.DeleteAllAsync();
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            var log = await _logRepository.GetByIdAsync(id);
            if (log == null)
                throw new KeyNotFoundException($"ID {id} ile Kayıt bulunamadı.");

            await _logRepository.DeleteAsync(log);
        }

        public Task<List<LogDto>> GetAllLogsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<LogDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var log = await _logRepository.GetByIdAsync(id);
            if (log == null)
            {
                return null;
            }
            var logDto = _mapper.Map<LogDto>(log);
            return logDto;
        }

        public Task<PagedResult<LogDto>> GetLogsAsync(int page, string term)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResult<LogDto>> GetPagedAsync(GridCommand gridCommand)
        {
            var log = await _logRepository.GetPagedAsync(gridCommand);
            return new PagedResult<LogDto>
            {
                Items = _mapper.Map<IEnumerable<LogDto>>(log.Items),
                TotalCount = log.TotalCount,
                PageNumber = log.PageNumber,
                PageSize = log.PageSize
            };
        }

        public Task<LogDto> UpdateAsync(UpdateLogDto model)
        {
            throw new NotImplementedException();
        }
    }
}
