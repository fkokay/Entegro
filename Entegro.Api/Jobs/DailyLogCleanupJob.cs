using Entegro.Application.Interfaces.Services.Base;
using Quartz;

namespace Entegro.Api.Jobs
{
    [DisallowConcurrentExecution]
    public class DailyLogCleanupJob : IJob
    {
        private readonly ILogService _logService;
        private readonly ILogger<DailyLogCleanupJob> _logger;
        public DailyLogCleanupJob(ILogger<DailyLogCleanupJob> logger, ILogService logService)
        {

            _logger = logger;
            _logService = logService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.LogInformation("DailyLogCleanupJob başladı.");
                await _logService.DeleteAllAsync();
                _logger.LogInformation("DailyLogCleanupJob tamamlandı.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while executing DailyLogCleanupJob.");

            }
        }
    }
}
