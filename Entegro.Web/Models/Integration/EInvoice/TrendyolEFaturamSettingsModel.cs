namespace Entegro.Web.Models.Integration.EInvoice
{
    public class TrendyolEFaturamSettingsModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool Active { get; set; }
        public int IntegrationSystemTypeId { get; set; }
        public int IntegrationSystemId { get; set; }
        public string EInvoiceType { get; set; }
        public string ApiUrl { get; set; }
        public string ApiUser { get; set; }
        public string ApiPassword { get; set; }
    }
}
