using Entegro.Api.Jobs;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace Entegro.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class XmlJobController : ControllerBase
    {
        private readonly ISchedulerFactory _schedulerFactory;

        public XmlJobController(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }

        [HttpPost("run")]
        public async Task<IActionResult> RunJob([FromBody] XmlJobRequest request)
        {
            if (string.IsNullOrEmpty(request.XmlUrl) || string.IsNullOrEmpty(request.UploadedFileName))
                return BadRequest("XmlUrl ve UploadedFileName zorunludur.");

            var scheduler = await _schedulerFactory.GetScheduler();

            var job = JobBuilder.Create<XmlFileDownloadJob>()
                .WithIdentity(Guid.NewGuid().ToString(), "XmlJobs")
                .UsingJobData("XmlUrl", request.XmlUrl)
                .UsingJobData("FolderName", request.FolderName ?? "")
                .UsingJobData("UploadedFileName", request.UploadedFileName)
                .Build();

            var trigger = TriggerBuilder.Create()
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(job, trigger);

            return Ok("Job tetiklendi.");
        }
    }

    public class XmlJobRequest
    {
        public string XmlUrl { get; set; }
        public string FolderName { get; set; }
        public string UploadedFileName { get; set; }
    }
}

