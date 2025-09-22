namespace Entegro.Web.Models.Integration.Marketplace
{
    public class PazaramaMarketplaceSettingsModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int IntegrationSystemTypeId { get; set; }
        public int IntegrationSystemId { get; set; }
        public string MarketplaceType { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }


}
