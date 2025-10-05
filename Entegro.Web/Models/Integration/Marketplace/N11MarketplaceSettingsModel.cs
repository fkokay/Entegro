namespace Entegro.Web.Models.Integration.Marketplace
{
    public class N11MarketplaceSettingsModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool Active { get; set; }
        public int IntegrationSystemTypeId { get; set; }
        public int IntegrationSystemId { get; set; }
        public string MarketplaceType { get; set; }
        public string AppSecret { get; set; }
        public string AppKey { get; set; }
    }
}
