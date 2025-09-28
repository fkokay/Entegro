using Entegro.Application.DTOs.TaskDescriptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.TaskExecutionInfo
{
    public class TaskExecutionInfoDto
    {
        public int Id { get; set; }
        public int TaskDescriptorId { get; set; }
        public bool IsRunning { get; set; }
        public string MachineName { get; set; }
        public DateTime StartedOn { get; set; }
        public DateTime? FinishedOn { get; set; }
        public DateTime? SucceededOn { get; set; }
        public string Error { get; set; }
        public int? ProgressPercent { get; set; }
        public string ProgressMessage { get; set; }
    }
}
