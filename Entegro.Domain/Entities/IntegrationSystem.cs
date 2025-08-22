using Entegro.Domain.Common;
using Entegro.Domain.Enums;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entegro.Domain.Entities
{
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


        public ICollection<IntegrationSystemParameter> _parameters;

        public ICollection<IntegrationSystemParameter> Parameters
        {
            get => LazyLoader?.Load(this, ref _parameters) ?? (_parameters ??= new HashSet<IntegrationSystemParameter>());
            set => _parameters = value;
        }
        public ICollection<IntegrationSystemLog> _logs;

        public ICollection<IntegrationSystemLog> Logs
        {
            get => LazyLoader?.Load(this, ref _logs) ?? (_logs ??= new HashSet<IntegrationSystemLog>());
            set => _logs = value;
        }

    }
}
