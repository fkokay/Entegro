using Entegro.Web.Models.Integration.Common;
using Entegro.Web.Models.Integration.Marketplace;

namespace Entegro.Web.Models.Integration
{
    public class MarketplaceListViewModel
    {
        public List<MarketplaceIntegrationSystemViewModel> MarketplaceList { get; set; } = new();
    }
}
