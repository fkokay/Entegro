namespace Entegro.Web.Models.Integration
{
    public class SmartstoreProductIntegrationModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductMainPicture { get; set; }
        public int IntegrationSystemId { get; set; }
        public int? ProductVariantAttributeCombinationId { get; set; }
        public string IntegrationSystemName { get; set; }
        public string CommerceType { get; set; }
        public string IntegrationCode { get; set; }
        public decimal Price { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public bool Active { get; set; } = true;
        public decimal CostPrice { get; set; }
        public decimal? Percent { get; set; }//yüzde kar oranı
        public decimal? ShippingFee { get; set; }//kargo
        public decimal? CommissionPercent { get; set; }//komisyon yüzdesi
        public decimal? ExtraCost { get; set; }//ekstra maliyet
        public bool ApplyAutoPrice { get; set; }//otomatik fiyat uygulansın mı

        public SmartstoreProductIntegrationCustomModel Custom { get; set; } = new SmartstoreProductIntegrationCustomModel();
    }

    public class SmartstoreProductIntegrationCustomModel
    {
        public int ManageInventoryMethod { get; set; }
        public int StockQunatity { get; set; }
        public bool DisplayStockAvailability { get; set; }
        public bool DisplayStockQuantity { get; set; }
        public int MinStockQuantity { get; set; }
        public int LowStockActivityId { get; set; }
        public bool ShowOnHomePage { get; set; }
        public int HomePageDisplayOrder { get; set; }
        public decimal? SpecialPrice { get; set; }
        public DateTime? SpecialPriceStartDateTime { get; set; }
        public DateTime? SpecialPriceEndDateTime { get; set; }
    }
}
