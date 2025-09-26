using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.N11
{
    public class N11BillingAddressDto
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("district")]
        public string District { get; set; }

        [JsonProperty("neighborhood")]
        public string Neighborhood { get; set; }

        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("gsm")]
        public string Gsm { get; set; }

        [JsonProperty("tcId")]
        public string TcId { get; set; }

        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }

        [JsonProperty("taxId")]
        public object TaxId { get; set; }

        [JsonProperty("taxHouse")]
        public object TaxHouse { get; set; }

        [JsonProperty("invoiceType")]
        public int InvoiceType { get; set; }
    }
}
