using Entegro.Application.DTOs.TaskExecutionInfo;
using Entegro.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.TaskDescriptor
{
    public class TaskDescriptorDto
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string CronExpression { get; set; }
        public string Type { get; set; }
        public bool Enabled { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.Normal;
        public bool StopOnError { get; set; }
        public DateTime? NextRunUtc { get; set; }
        public bool IsHidden { get; set; }
        public bool RunPerMachine { get; set; }
        public bool IsPending
            => Enabled
               && NextRunUtc.HasValue
               && NextRunUtc <= DateTime.UtcNow
               && (LastExecution == null || !LastExecution.IsRunning);
        public TaskExecutionInfoDto? LastExecution { get; set; }
        public ICollection<TaskExecutionInfoDto> ExecutionHistory { get; set; } = new List<TaskExecutionInfoDto>();
    }
}
