using Entegro.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities
{
    [Table("IntegrationSystemLog")]
    public class IntegrationSystemLog : BaseEntity
    {
        public int IntegrationSystemId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
        public string LogLevel { get; set; } // e.g., "Info", "Warning", "Error"
        public string? Exception { get; set; } // Optional exception details
        public virtual IntegrationSystem IntegrationSystem { get; set; }
    }
}
