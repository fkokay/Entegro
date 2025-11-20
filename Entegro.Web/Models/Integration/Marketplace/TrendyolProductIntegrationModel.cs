namespace Entegro.Web.Models.Integration.Marketplace
{
    public class TrendyolProductIntegrationModel
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
        public decimal CostPrice { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public bool Active { get; set; } = true;
        public decimal? Percent { get; set; }//yüzde kar oranı
        public decimal? ShippingFee { get; set; }//kargo
        public decimal? CommissionPercent { get; set; }//komisyon yüzdesi
        public decimal? ExtraCost { get; set; }//ekstra maliyet
        public bool ApplyAutoPrice { get; set; }//otomatik fiyat uygulansın mı
        public TrednyolProductIntegrationCustomModel Custom { get; set; } = new TrednyolProductIntegrationCustomModel();
    }

    public class TrednyolProductIntegrationCustomModel
    {
    }

}
