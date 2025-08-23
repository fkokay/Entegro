namespace Entegro.Application.DTOs.ProductIntegration
{
    public class CreateProductIntegrationDto
    {
        public int ProductId { get; set; }
        public int IntegrationSystemId { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public decimal Price { get; set; }
        public bool Active { get; set; }
    }
}
