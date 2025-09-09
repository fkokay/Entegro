namespace Entegro.Application.DTOs.ProductIntegration
{
    public class UpdateProductIntegrationDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int? ProductVariantAttributeCombinationId { get; set; }
        public int IntegrationSystemId { get; set; }
        public string IntegrationCode { get; set; }
        public decimal Price { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public bool IsSync { get; set; }
        public string? Custom { get; set; }
        public bool Active { get; set; }
      
    }
}
