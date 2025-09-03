namespace Entegro.Web.Models
{
    public class SmartstoreProductIntegrationViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductMainPicture { get; set; }
        public int IntegrationSystemId { get; set; }
        public string IntegrationCode { get; set; }
        public decimal Price { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public bool Active { get; set; } = true;

        public SmartstoreProductIntegrationCustomViewModel Custom { get; set; } = new SmartstoreProductIntegrationCustomViewModel();
    }

    public class SmartstoreProductIntegrationCustomViewModel
    {
        public int ManageInventoryMethod { get; set; }
    }
}
