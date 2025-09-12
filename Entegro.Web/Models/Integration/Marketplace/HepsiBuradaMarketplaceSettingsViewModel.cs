namespace Entegro.Web.Models.Integration.Marketplace
{
    public class HepsiBuradaMarketplaceSettingsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int IntegrationSystemTypeId { get; set; }
        public int IntegrationSystemId { get; set; }
        public string MarketplaceType { get; set; }
        public string ApiUser { get; set; }
        public string ApiPassword { get; set; }
        public string MerchantId { get; set; }
        public string UserAgent { get; set; }
    }
}
