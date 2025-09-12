using Entegro.Application.DTOs.Marketplace.Trendyol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.Pazarama
{
    public class PazaramaStockUpdateDto
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("listPrice")]
        public decimal ListPrice { get; set; }
        [JsonPropertyName("salePrice")]
        public decimal SalePrice { get; set; }
    }

    public class PazaramaStockUpdateRequest
    {
        [JsonPropertyName("items")]
        public List<PazaramaStockUpdateDto> Items { get; set; } = new List<PazaramaStockUpdateDto>();
    }
}
