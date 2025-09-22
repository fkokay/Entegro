using Entegro.Domain.Enums;

namespace Entegro.Web.Models.Integration.EInvoice
{
    public class EInvoiceIntegrationSystemModel
    {
        public int Id { get; set; }
        public int IntegrationSystemTypeId { get; set; }
        public IntegrationSystemType IntegrationSystemType { get; set; }
        public string IntegrationSystemTypeLabelHint { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string EInvoiceType { get; set; }
    }
}
