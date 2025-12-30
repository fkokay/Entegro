using Entegro.Api.Jobs;
using Entegro.Application.DTOs.Job;
using Entegro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Quartz;
using Quartz.Impl.Matchers;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace Entegro.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly ILogger<JobController> _logger;

        public JobController(ISchedulerFactory schedulerFactory, ILogger<JobController> logger)
        {
            _schedulerFactory = schedulerFactory;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Run(string type, int taskId, int? parameter)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(type))
                    return BadRequest("Type parametresi boş olamaz.");

                var runId = Guid.NewGuid();
                var scheduler = await _schedulerFactory.GetScheduler();

                var jobType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => (t.Name == type || t.FullName == type) && typeof(IJob).IsAssignableFrom(t));
                if (jobType == null)
                {
                    _logger.LogError("Type bulunamadı veya IJob değil: {Type}", type);
                    return StatusCode(500, $"Type bulunamadı veya IJob değil: {type}");
                }
                var jobKey = new JobKey(taskId.ToString(), "AdHocJobs");

                var executingJobs = await scheduler.GetCurrentlyExecutingJobs();
                if (executingJobs.Any(j => j.JobDetail.Key.Equals(jobKey)))
                {
                    _logger.LogError("{Type} job zaten çalışıyor.", type);
                    return StatusCode(500, $"{type} job zaten çalışıyor.");
                }

                var job = JobBuilder.Create(jobType).WithIdentity(jobKey)
                    .UsingJobData("RunId", runId.ToString()).Build();


                if (parameter.HasValue)
                    job.JobDataMap["Parameter"] = parameter.Value;


                var trigger = TriggerBuilder.Create()
                                  .WithIdentity($"{taskId}-trigger", "AdHocTriggers")
                                  .StartNow()
                                  .Build();

                await scheduler.ScheduleJob(job, trigger);

                return Ok(new { Success = true, Job = jobType.Name, RunId = runId, TriggerTime = DateTime.Now, Parameter = parameter.HasValue ? parameter : null });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Job tetiklenirken hata oluştu: {Type}", type);
                return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
            }

        }

        [HttpPost]
        public async Task<IActionResult> Stop(string type)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(type))
                    return BadRequest("Type parametresi boş olamaz.");

                var scheduler = await _schedulerFactory.GetScheduler();

                var jobType = Assembly.GetExecutingAssembly().GetTypes()
                    .FirstOrDefault(t => (t.Name == type || t.FullName == type) && typeof(IJob).IsAssignableFrom(t));

                if (jobType == null)
                {
                    _logger.LogWarning("{Type} job bulunamadı.", type);
                    return NotFound($"{type} job bulunamadı.");
                }

                var jobKey = new JobKey(jobType.Name, "CustomJobs");

                if (!await scheduler.CheckExists(jobKey))
                {
                    _logger.LogWarning("{Type} job tanımlı değil.", type);
                    return NotFound($"{type} job tanımlı değil.");
                }

                await scheduler.DeleteJob(jobKey);

                return Ok(new { Success = true, Message = $"{type} job durduruldu." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Job durdurulurken hata oluştu. Type: {Type}", type);
                return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
            }
        }


        [HttpPost]
        public async Task<IActionResult> Pause(string type)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(type))
                    return BadRequest("Type parametresi boş olamaz.");

                var scheduler = await _schedulerFactory.GetScheduler();

                var jobType = Assembly.GetExecutingAssembly().GetTypes()
                    .FirstOrDefault(t => (t.Name == type || t.FullName == type) && typeof(IJob).IsAssignableFrom(t));

                if (jobType == null)
                {
                    _logger.LogWarning("{Type} job bulunamadı.", type);
                    return NotFound($"{type} job bulunamadı.");
                }

                var jobKey = new JobKey(jobType.Name, "CustomJobs");

                if (!await scheduler.CheckExists(jobKey))
                {
                    _logger.LogWarning("{Type} job tanımlı değil.", type);
                    return NotFound($"{type} job tanımlı değil.");
                }

                await scheduler.PauseJob(jobKey);

                return Ok(new { Success = true, Message = $"{type} job duraklatıldı." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Job duraklatılırken hata oluştu: {Type}", type);
                return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Resume(string type)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(type))
                    return BadRequest("Type parametresi boş olamaz.");

                var scheduler = await _schedulerFactory.GetScheduler();

                var jobType = Assembly.GetExecutingAssembly().GetTypes()
                    .FirstOrDefault(t => (t.Name == type || t.FullName == type) && typeof(IJob).IsAssignableFrom(t));

                if (jobType == null)
                {
                    _logger.LogWarning("{Type} job bulunamadı.", type);
                    return NotFound($"{type} job bulunamadı.");
                }

                var jobKey = new JobKey(jobType.Name, "CustomJobs");

                if (!await scheduler.CheckExists(jobKey))
                {
                    _logger.LogWarning("{Type} job tanımlı değil.", type);
                    return NotFound($"{type} job tanımlı değil.");
                }

                await scheduler.ResumeJob(jobKey);

                return Ok(new { Success = true, Message = $"{type} job yeniden başlatıldı." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Job yeniden başlatılırken hata oluştu: {Type}", type);
                return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<JobInfoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> List()
        {
            try
            {
                var scheduler = await _schedulerFactory.GetScheduler();
                var executingJobs = await scheduler.GetCurrentlyExecutingJobs();
                var jobs = new List<JobInfoDto>();

                foreach (var group in await scheduler.GetJobGroupNames())
                {
                    foreach (var jobKey in await scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(group)))
                    {
                        var detail = await scheduler.GetJobDetail(jobKey);
                        var triggers = await scheduler.GetTriggersOfJob(jobKey);

                        var triggerInfos = new List<TriggerInfoDto>();

                        foreach (var trigger in triggers)
                        {
                            var state = await scheduler.GetTriggerState(trigger.Key);

                            triggerInfos.Add(new TriggerInfoDto
                            {
                                TriggerKey = trigger.Key.ToString(),
                                NextFireTime = trigger.GetNextFireTimeUtc()?.LocalDateTime,
                                PreviousFireTime = trigger.GetPreviousFireTimeUtc()?.LocalDateTime,
                                State = state.ToString()
                            });
                        }

                        var isRunning = executingJobs.Any(j => j.JobDetail.Key.Equals(jobKey));

                        string overallStatus;
                        if (isRunning)
                            overallStatus = "Running";
                        else if (triggerInfos.Any(t => t.State == "Error"))
                            overallStatus = "Error";
                        else if (triggerInfos.Any(t => t.State == "Paused"))
                            overallStatus = "Paused";
                        else if (triggerInfos.Any(t => t.State == "Complete"))
                            overallStatus = "Complete";
                        else
                            overallStatus = "Idle";

                        var runId = detail.JobDataMap.ContainsKey("RunId") ? detail.JobDataMap.GetString("RunId") : null;

                        jobs.Add(new JobInfoDto
                        {
                            JobName = jobKey.Name,
                            JobGroup = jobKey.Group,
                            JobType = detail.JobType.FullName,
                            Triggers = triggerInfos,
                            Status = overallStatus,
                            RunId = runId
                        });
                    }
                }

                return Ok(jobs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Job listelenirken hata oluştu.");
                return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
            }
        }



    }
}

