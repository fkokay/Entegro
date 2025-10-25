using Entegro.Application.DTOs.Product;

namespace Entegro.Application.DTOs.OrderItem
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int? ProductId { get; set; }
        public string Sku { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Price { get; set; }
        public decimal TaxRate { get; set; }
        public decimal DiscountAmount { get; set; }
        public string? AttributesXml { get; set; }
        public string? AttributesDescription { get; set; }
        public decimal ItemWeight { get; set; }
        public decimal ProductCost { get; set; }
        public string? IntegrationSku { get; set; }
        public string? IntegrationProductName { get; set; }
        public string? IntegrationProductImageUrl { get; set; }//pazaryerinden gelen ve eşleştirilmeyen ürün resmi
        public ProductDto? Product { get; set; }
    }
}
