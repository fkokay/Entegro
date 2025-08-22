using Entegro.Domain.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
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

        private IntegrationSystem? _integrationSystem;
        public IntegrationSystem? IntegrationSystem
        {
            get => _integrationSystem ?? LazyLoader?.Load(this, ref _integrationSystem);
            set => _integrationSystem = value;
        }

    }
}
