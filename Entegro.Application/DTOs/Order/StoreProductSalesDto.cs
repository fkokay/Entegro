namespace Entegro.Application.DTOs.Order
{
    public class StoreProductSalesDto
    {
        public string StoreName { get; set; }
        public string IntegrationSku { get; set; }
        public string IntegrationProductName { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Barcode { get; set; }
        public decimal TotalQuantity { get; set; }
    }

}
