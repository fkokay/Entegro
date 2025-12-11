namespace Entegro.Application.DTOs.Erp
{
    public class ErpOrderItemDto
    {
        public string OrderNumber { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string BrandName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Price { get; set; }
        public decimal VatRate { get; set; }
        public decimal DiscountAmount { get; set; }
    }
}

