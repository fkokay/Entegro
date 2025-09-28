using Entegro.Infrastructure.Data;
using Entegro.Scheduling;
using Quartz;
using CronExpression = Entegro.Scheduling.CronExpression;

namespace Entegro.Api.Services
{
    public class TaskExecutionListener : IJobListener
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public TaskExecutionListener(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public string Name => "TaskExecutionListener";

        public async Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<EntegroDbContext>();

            var execInfo = new TaskExecutionInfo
            {
                MachineName = Environment.MachineName,
                TaskDescriptorId = int.Parse(context.JobDetail.Key.Name),
                StartedOnUtc = DateTime.UtcNow,
                IsRunning = true
            };

            db.TaskExecutionInfos.Add(execInfo);
            await db.SaveChangesAsync(cancellationToken);

            context.MergedJobDataMap["ExecutionInfoId"] = execInfo.Id;
        }

        public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException, CancellationToken cancellationToken = default)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<EntegroDbContext>();

            var taskId = int.Parse(context.JobDetail.Key.Name);
            var task = await db.TaskDescriptors.FindAsync(taskId);

            if (task is not null)
            {
                var execInfoId = (int)context.MergedJobDataMap["ExecutionInfoId"];
                var execInfo = await db.TaskExecutionInfos.FindAsync(execInfoId);

                if (execInfo is not null)
                {
                    execInfo.IsRunning = false;
                    execInfo.FinishedOnUtc = DateTime.UtcNow;
                    execInfo.SucceededOnUtc = jobException == null ? DateTime.UtcNow : null;
                    execInfo.Error = jobException?.Message;
                }

                if (!string.IsNullOrEmpty(task.CronExpression))
                {
                    var next = CronExpression.GetNextSchedule(task.CronExpression, DateTime.UtcNow);
                    task.NextRunUtc = next;
                }
                else
                {
                    task.NextRunUtc = null;
                }

                await db.SaveChangesAsync(cancellationToken);
            }
        }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
