namespace Entegro.Web.Models
{
    public class TrendyolMarketplaceSettingsViewModel
    {
        //mağaza bilgileri
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int IntegrationSystemTypeId { get; set; }
        //entegro entegrasyon sistemi id
        public int IntegrationSystemId { get; set; }
        public string CommerceType { get; set; }
        public string ApiUser { get; set; }
        public string ApiPassword { get; set; }
        public string SupplierId { get; set; }
    }

}
