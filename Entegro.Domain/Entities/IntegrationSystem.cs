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


        private ICollection<IntegrationSystemParameter> _integrationSystemParameters;
        public ICollection<IntegrationSystemParameter> IntegrationSystemParameters
        {
            get => LazyLoader?.Load(this, ref _integrationSystemParameters) ?? (_integrationSystemParameters ??= new HashSet<IntegrationSystemParameter>());
            set => _integrationSystemParameters = value;
        }

        private ICollection<IntegrationSystemLog> _integrationSystemLogs;
        public ICollection<IntegrationSystemLog> IntegrationSystemLogs
        {
            get => LazyLoader?.Load(this, ref _integrationSystemLogs) ?? (_integrationSystemLogs ??= new HashSet<IntegrationSystemLog>());
            set => _integrationSystemLogs = value;
        }

    }
}
