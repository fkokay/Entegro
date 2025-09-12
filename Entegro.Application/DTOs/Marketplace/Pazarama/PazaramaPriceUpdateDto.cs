using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.Pazarama
{
    public class PazaramaPriceUpdateDto
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("listPrice")]
        public decimal ListPrice { get; set; }
        [JsonPropertyName("salePrice")]
        public decimal SalePrice { get; set; }
    }

    public class PazaramaPriceUpdateRequest
    {
        [JsonPropertyName("items")]
        public List<PazaramaPriceUpdateDto> Items { get; set; } = new List<PazaramaPriceUpdateDto>();
    }
}
