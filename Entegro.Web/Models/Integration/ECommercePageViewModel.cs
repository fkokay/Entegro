namespace Entegro.Web.Models.Integration
{
    public class ECommercePageViewModel
    {
        public IntegrationSystemViewModel CurrentEcommerce { get; set; }
        public List<IntegrationSystemViewModel> MyEcommerceList { get; set; } = new();
    }
}
