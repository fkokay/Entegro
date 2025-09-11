namespace Entegro.Web.Models.Integration.Marketplace
{
    public class CicekSepetiProductIntegrationViewModel
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

        public CicekSepetiProductIntegrationCustomViewModel Custom { get; set; } = new CicekSepetiProductIntegrationCustomViewModel();
    }
    public class CicekSepetiProductIntegrationCustomViewModel
    {
    }

}
