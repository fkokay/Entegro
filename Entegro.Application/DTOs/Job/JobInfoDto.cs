using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Job
{
    public class JobInfoDto
    {
        public string JobName { get; set; }
        public string JobGroup { get; set; }
        public string JobType { get; set; }
        public List<TriggerInfoDto> Triggers { get; set; }
        public string Status { get; set; }
        public string? RunId { get; set; }
    }
}
