namespace Entegro.Application.DTOs.OrderItem
{
    public class OrderItemListDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductMainPicture { get; set; }
        public int? ProductMainPictureId { get; set; }
        public string ProductCode { get; set; }
        public string? ProductBarcode { get; set; }
        public int Quantity { get; set; }
        public string AttributeDescription { get; set; }
        public decimal UnitPrice { get; set; }
        public string? IntegrationProductName { get; set; }
        public string? IntegrationSku { get; set; }
        public string? IntegrationProductImageUrl { get; set; }
    }
}
