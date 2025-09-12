using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.Trendyol
{
    public class TrendyolPriceAndStockUpdateDto
    {
        [JsonPropertyName("barcode")]
        public string Barcode { get; set; }
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
        [JsonPropertyName("salePrice")]
        public decimal SalePrice { get; set; }
        [JsonPropertyName("listPrice")]
        public decimal ListPrice { get; set; }
    }

    public class TrendyolPriceAndStockUpdateRequest
    {
        [JsonPropertyName("items")]
        public List<TrendyolPriceAndStockUpdateDto> Items { get; set; } = new List<TrendyolPriceAndStockUpdateDto>();
    }
}
