namespace Entegro.Web.Models
{
    public class CreateProductIntegrationViewModel
    {

        public int Id { get; set; }
        public int ProductId { get; set; }
        public int IntegrationSystemId { get; set; }
        public string IntegrationCode { get; set; }
        public decimal Price { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public bool Active { get; set; } = true;


        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductMainPicture { get; set; }
    }
}
