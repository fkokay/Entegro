using Entegro.Domain.Common;
using Entegro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    IntegrationSystemType.EInvoice =>"E-Fatura Entegrasyonu",
                };
            }
        }
        public string Name { get; set; }
        public string? Description { get; set; } 

        public virtual ICollection<IntegrationSystemParameter> Parameters { get; set; } = new List<IntegrationSystemParameter>();
        public virtual ICollection<IntegrationSystemLog> Logs { get; set; } = new List<IntegrationSystemLog>();
    }
}
