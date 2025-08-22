namespace Entegro.Application.DTOs.ProductIntegration
{
    public class UpdateProductIntegrationDto
    {
        public int ProductId { get; set; }
        public int IntegrationSystemId { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public bool Active { get; set; }
    }
}
