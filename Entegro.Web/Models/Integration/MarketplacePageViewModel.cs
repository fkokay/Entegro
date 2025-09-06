namespace Entegro.Web.Models.Integration
{
    public class MarketplacePageViewModel
    {
        public IntegrationSystemViewModel CurrentMarketplace { get; set; }
        public List<IntegrationSystemViewModel> MyMarketplaceList { get; set; } = new();
    }
}
