using Entegro.Domain.Enums;

namespace Entegro.Web.Models.Integration.Marketplace
{
    public class MarketplaceIntegrationSystemModel
    {
        public int Id { get; set; }
        public int IntegrationSystemTypeId { get; set; }
        public IntegrationSystemType IntegrationSystemType { get; set; }
        public string IntegrationSystemTypeLabelHint { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool Active { get; set; }
        public string MarketplaceType { get; set; }
    }
}
