using Entegro.Web.Models.Integration.Common;
using Entegro.Web.Models.Integration.Marketplace;

namespace Entegro.Web.Models.Integration
{
    public class MarketplaceListModel
    {
        public List<MarketplaceIntegrationSystemModel> MarketplaceList { get; set; } = new();
    }
}
