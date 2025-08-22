namespace Entegro.Application.DTOs.ProductIntegration
{
    public class CreateProductIntegrationDto
    {
        public int IntegrationSystemId { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public bool Active { get; set; }
    }
}
