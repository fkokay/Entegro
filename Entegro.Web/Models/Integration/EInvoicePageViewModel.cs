namespace Entegro.Web.Models.Integration
{
    public class EInvoicePageViewModel
    {
        public IntegrationSystemViewModel CurrentEInvoice { get; set; }
        public List<IntegrationSystemViewModel> MyEInvoiceList { get; set; } = new();
    }
}
