using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Job
{
    public class TriggerInfoDto
    {
        public string TriggerKey { get; set; }
        public DateTime? NextFireTime { get; set; }
        public DateTime? PreviousFireTime { get; set; }
        public string State { get; set; }
    }
}
