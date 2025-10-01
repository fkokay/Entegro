using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.EInvoice.EFinans
{
    public class InvoiceNumberRequest
    {
        public Guid Uuid { get; set; } = Guid.NewGuid();
        public string TaxNumber { get; set; } = string.Empty;
        public string Branch { get; set; } = "01";
        public string Cashier { get; set; } = "01";
        public string InvoiceCode { get; set; } = string.Empty;
        public bool IsEInvoice { get; set; } = true;
    }
}
