namespace Entegro.Web.Models.Integration.Marketplace
{
    public class N11ProductIntegrationViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int? ProductVariantAttributeCombinationId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductMainPicture { get; set; }
        public int IntegrationSystemId { get; set; }
        public string IntegrationSystemName { get; set; }
        public string MarketplaceType { get; set; }
        public string MarketplaceLink { get; set; }
        public string IntegrationCode { get; set; }
        public decimal Price { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public bool Active { get; set; } = true;

        public N11ProductIntegrationCustomViewModel Custom { get; set; } = new N11ProductIntegrationCustomViewModel();
    }
    public class N11ProductIntegrationCustomViewModel
    {
    }

}
