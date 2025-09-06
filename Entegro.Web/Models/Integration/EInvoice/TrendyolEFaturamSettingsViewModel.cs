namespace Entegro.Web.Models.Integration.EInvoice
{
    public class TrendyolEFaturamSettingsViewModel
    {
        //mağaza bilgileri
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int IntegrationSystemTypeId { get; set; }

        //entegro entegrasyon 
        public int IntegrationSystemId { get; set; }
        public string CommerceType { get; set; }
        public string ApiUrl { get; set; }
        public string ApiUser { get; set; }
        public string ApiPassword { get; set; }
    }
}
