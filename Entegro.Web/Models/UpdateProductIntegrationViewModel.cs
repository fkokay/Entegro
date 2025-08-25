namespace Entegro.Web.Models
{
    public class UpdateProductIntegrationViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int IntegrationSystemId { get; set; }
        public string IntegrationCode { get; set; }
        public decimal Price { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public bool Active { get; set; } = true;
    }
}
