
using Autofac;
using Entegro.Infrastructure.Data;
using Entegro.Scheduling;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.Impl;
using System;
using System.Reflection;
using CronExpression = Entegro.Scheduling.CronExpression;

namespace Entegro.Api.Services
{
    public class QuartzSchedulerService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly ILogger<QuartzSchedulerService> _logger;
        private IScheduler _scheduler;

        public QuartzSchedulerService(
            IServiceScopeFactory scopeFactory,
            ISchedulerFactory schedulerFactory,
            ILogger<QuartzSchedulerService> logger)
        {
            _scopeFactory = scopeFactory;
            _schedulerFactory = schedulerFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _scheduler = await _schedulerFactory.GetScheduler(stoppingToken);
            _scheduler.ListenerManager.AddJobListener(new TaskExecutionListener(_scopeFactory));
            await _scheduler.Start(stoppingToken);

            _logger.LogInformation("QuartzSchedulerService başlatıldı.");

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<EntegroDbContext>();

                var tasks = await db.TaskDescriptors
                    .Where(t => t.Enabled)
                    .OrderByDescending(t => t.Priority)
                    .ToListAsync(stoppingToken);

                foreach (var task in tasks)
                {
                    try
                    {
                        var jobKey = new JobKey(task.Id.ToString(), "CustomJobs");

                        if (await _scheduler.CheckExists(jobKey, stoppingToken))
                            continue;

                        var executingJobs = await _scheduler.GetCurrentlyExecutingJobs();
                        if (executingJobs.Any(j => j.JobDetail.Key.Equals(jobKey)))
                        {
                            _logger.LogError("{Type} job zaten çalışıyor.", task.Type);
                            continue;
                        }

                        var type = Assembly.GetExecutingAssembly().GetTypes()
                        .FirstOrDefault(t => (t.Name == task.Type || t.FullName == task.Type) && typeof(IJob).IsAssignableFrom(t));
                        if (type == null)
                        {
                            _logger.LogError("Task {Name} için type bulunamadı veya IJob değil: {Type}", task.Name, task.Type);
                            continue;
                        }

                        var job = JobBuilder.Create(type)
                            .WithIdentity(jobKey)
                            .WithDescription(task.Name)
                            .Build();

                        ITrigger trigger;
                        if (!string.IsNullOrWhiteSpace(task.CronExpression))
                        {
                            trigger = TriggerBuilder.Create()
                                .WithIdentity($"{task.Id}-trigger", "CustomTriggers")
                                .WithCronSchedule(CronConverter.ToQuartz(task.CronExpression))
                                .Build();
                        }
                        else
                        {
                            trigger = TriggerBuilder.Create()
                                .WithIdentity($"{task.Id}-trigger", "CustomTriggers")
                                .StartNow()
                                .Build();
                        }

                        await _scheduler.ScheduleJob(job, trigger, stoppingToken);

                        _logger.LogInformation("Task {Name} Quartz’a eklendi. Cron={Cron}", task.Name, task.CronExpression);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Task {Name} eklenirken hata", task.Name);
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }

    public static class CronConverter
    {
        /// <summary>
        /// Linux tarzı (5 alan) cron ifadesini Quartz uyumlu (6 alan) cron ifadesine çevirir.
        /// </summary>
        /// <param name="linuxCron">Örn: "* * * * *"</param>
        /// <returns>Quartz cron string (6 alan)</returns>
        public static string ToQuartz(string linuxCron)
        {
            if (string.IsNullOrWhiteSpace(linuxCron))
                throw new ArgumentException("Cron ifadesi boş olamaz", nameof(linuxCron));

            var parts = linuxCron.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 5)
                throw new ArgumentException($"Geçersiz cron formatı: {linuxCron}. Linux cron 5 parçalı olmalı.", nameof(linuxCron));

            // Linux cron formatı: dakika, saat, günAy, ay, günHafta
            var minute = parts[0];
            var hour = parts[1];
            var dayOfMonth = parts[2];
            var month = parts[3];
            var dayOfWeek = parts[4];

            // Quartz: saniye + dakika + saat + günAy + ay + günHafta
            // Eğer Linux tarafında dayOfWeek '*' ise Quartz tarafında '?' koymamız gerekir.
            var quartzDayOfWeek = dayOfWeek == "*" ? "?" : dayOfWeek;

            return $"0 {minute} {hour} {dayOfMonth} {month} {quartzDayOfWeek}";
        }
    }
}
