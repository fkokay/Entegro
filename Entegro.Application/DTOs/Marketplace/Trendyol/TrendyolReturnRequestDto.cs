namespace Entegro.Application.DTOs.Marketplace.Trendyol
{
    public class TrendyolReturnRequestDto
    {
        public string orderNumber { get; set; }
        public long orderId { get; set; }
        public string barcode { get; set; }
        public string reason { get; set; }
        public string reasonDetail { get; set; }
        public string returnStatus { get; set; }
        public DateTime createdDate { get; set; }
        public int quantity { get; set; }
    }
}
