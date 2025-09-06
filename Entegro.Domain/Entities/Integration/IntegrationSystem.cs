using Entegro.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entegro.Domain.Entities.Integration
{
    public class IntegrationSystemMap : IEntityTypeConfiguration<IntegrationSystem>
    {
        public void Configure(EntityTypeBuilder<IntegrationSystem> builder)
        {

        }
    }
    [Table("IntegrationSystem")]
    public class IntegrationSystem : BaseEntity
    {
        public int IntegrationSystemTypeId { get; set; }

        [NotMapped]
        public IntegrationSystemType IntegrationSystemType
        {
            get => (IntegrationSystemType)IntegrationSystemTypeId;
            set => IntegrationSystemTypeId = (int)value;
        }

        [NotMapped]
        public string IntegrationSystemTypeLabelHint
        {
            get
            {
                return IntegrationSystemType switch
                {
                    IntegrationSystemType.None => "Yok",
                    IntegrationSystemType.ERP => "ERP Entegrasyonu",
                    IntegrationSystemType.Commerce => "E-Ticareti Entegrasyonu",
                    IntegrationSystemType.Marketplace => "Pazaryeri Entegrasyonu",
                    IntegrationSystemType.Cargo => "Kargo Entegrasyonu",
                    IntegrationSystemType.EInvoice => "E-Fatura Entegrasyonu",
                };
            }
        }
        public string Name { get; set; }
        public string? Description { get; set; }
        public virtual ICollection<IntegrationSystemParameter> IntegrationSystemParameters { get; set; } = new HashSet<IntegrationSystemParameter>();
        public virtual ICollection<IntegrationSystemLog> IntegrationSystemLogs { get; set; } = new HashSet<IntegrationSystemLog>();

    }
}
