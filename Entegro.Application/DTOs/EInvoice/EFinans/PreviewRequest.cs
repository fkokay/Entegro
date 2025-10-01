using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.EInvoice.EFinans
{
    public class PreviewRequest
    {
        public string TaxNumber { get; set; } = string.Empty;
        public string Branch { get; set; } = "01";
        public string Cashier { get; set; } = "01";
        public byte[] Document { get; set; } = [];
        public string Format { get; set; } = "UBL";
        public string DocumentType { get; set; } = "INVOICE";
        public string XsltName { get; set; } = "";
        public bool IsEInvoice { get; set; } = false;
    }
}
