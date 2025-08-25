using Entegro.Application.DTOs.IntegrationSystem;
using Entegro.Application.DTOs.Product;

namespace Entegro.Application.DTOs.ProductIntegration
{
    public class ProductIntegrationDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int IntegrationSystemId { get; set; }
        public string IntegrationCode { get; set; }
        public decimal Price { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public bool Active { get; set; }
        public ProductDto Product { get; set; }
        public IntegrationSystemDto IntegrationSystem { get; set; }
    }
}
