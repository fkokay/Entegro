using Entegro.Application.DTOs.IntegrationSystem;

namespace Entegro.Application.DTOs.ProductIntegration
{
    public class ProductIntegrationDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int? ProductVariantAttributeCombinationId { get; set; }
        public int IntegrationSystemId { get; set; }
        public string IntegrationCode { get; set; }
        public decimal Price { get; set; }
        public string Custom { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public bool IsSync { get; set; }
        public bool Active { get; set; }
        public decimal? Percent { get; set; }//yüzde kar oranı
        public decimal? ShippingFee { get; set; }//kargo
        public decimal? CommissionPercent { get; set; }//komisyon yüzdesi
        public decimal? ExtraCost { get; set; }//ekstra maliyet
        public bool ApplyAutoPrice { get; set; }
        public IntegrationSystemDto IntegrationSystem { get; set; }
    }
}
