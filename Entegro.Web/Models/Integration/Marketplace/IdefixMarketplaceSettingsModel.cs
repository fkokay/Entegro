namespace Entegro.Web.Models.Integration.Marketplace
{
    public class IdefixMarketplaceSettingsModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int IntegrationSystemTypeId { get; set; }
        public int IntegrationSystemId { get; set; }
        public string MarketplaceType { get; set; }
        public string Token { get; set; }
        public string Secret { get; set; }
        public string SellerId { get; set; }
    }
}
