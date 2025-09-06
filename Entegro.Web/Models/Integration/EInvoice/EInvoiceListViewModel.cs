using Entegro.Web.Models.Integration.Common;

namespace Entegro.Web.Models.Integration.EInvoice
{
    public class EInvoiceListViewModel
    {
        public List<EInvoiceIntegrationSystemViewModel> EInvoiceList { get; set; } = new();
    }
}
