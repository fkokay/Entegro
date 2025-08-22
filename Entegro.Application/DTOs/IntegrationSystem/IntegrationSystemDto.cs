using Entegro.Application.DTOs.IntegrationSystemParameter;
using Entegro.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Application.DTOs.IntegrationSystem
{
    public class IntegrationSystemDto
    {
        public int Id { get; set; }
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

        public List<IntegrationSystemParameterDto> Parameters { get; set; } = new();
    }
}
