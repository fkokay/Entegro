using Entegro.Scheduling;

namespace Entegro.Web.Models.Platform.Scheduling
{
    public class TaskDescriptorViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string CronExpression { get; set; }
        public string CronDescription { get; set; }
        public string Type { get; set; }
        public bool Enabled { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.Normal;
        public bool StopOnError { get; set; }
        public DateTime? NextRun { get; set; }
        public bool IsHidden { get; set; }
        public bool RunPerMachine { get; set; }
        public bool IsPending
            => Enabled
               && NextRun.HasValue
               && NextRun <= DateTime.Now
               && (LastExecution == null || !LastExecution.IsRunning);
        public TaskExecutionInfoViewModel? LastExecution { get; set; }
        public ICollection<TaskExecutionInfoViewModel> ExecutionHistory { get; set; } = new List<TaskExecutionInfoViewModel>();
    }
}
