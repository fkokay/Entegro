using Entegro.Api.Jobs;
using Entegro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace Entegro.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IImportProfileService _importProfileService;

        public JobController(ISchedulerFactory schedulerFactory, IImportProfileService importProfileService)
        {
            _schedulerFactory = schedulerFactory;
            _importProfileService = importProfileService;
        }

        [HttpPost("run")]
        public async Task<IActionResult> RunJob(int profileId)
        {
            try
            {
                var scheduler = await _schedulerFactory.GetScheduler();

                var job = JobBuilder.Create<XmlFileDownloadJob>()
                    .WithIdentity(Guid.NewGuid().ToString(), "XmlJobs")
                    .UsingJobData("ProfileId", profileId)
                    .Build();

                var trigger = TriggerBuilder.Create()
                    .StartNow()
                    .Build();

                await scheduler.ScheduleJob(job, trigger);

                return Ok("Job tetiklendi.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
            }

        }
    }
}

