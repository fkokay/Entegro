namespace Entegro.Web.Models
{
    public class UpdateProductIntegrationViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int IntegrationSystemId { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public bool Active { get; set; } = true;
        public decimal Price { get; set; }
    }
}
