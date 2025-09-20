using Entegro.Web.Models.Integration.Common;

namespace Entegro.Web.Models.Integration.EInvoice
{
    public class EInvoiceListModel
    {
        public List<EInvoiceIntegrationSystemModel> EInvoiceList { get; set; } = new();
    }
}
