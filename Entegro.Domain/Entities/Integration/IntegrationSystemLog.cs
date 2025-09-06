using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entegro.Domain.Entities.Integration
{
    public class IntegrationSystemLogMap : IEntityTypeConfiguration<IntegrationSystemLog>
    {
        public void Configure(EntityTypeBuilder<IntegrationSystemLog> builder)
        {

        }
    }
    [Table("IntegrationSystemLog")]
    public class IntegrationSystemLog : BaseEntity
    {
        public int IntegrationSystemId { get; set; }
        public virtual IntegrationSystem? IntegrationSystem { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
        public string LogLevel { get; set; } // e.g., "Info", "Warning", "Error"
        public string? Exception { get; set; } // Optional exception details
    }
}
