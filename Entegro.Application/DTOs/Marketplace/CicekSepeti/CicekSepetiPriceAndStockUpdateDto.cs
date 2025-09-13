using Entegro.Application.DTOs.Marketplace.N11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.CicekSepeti
{
    public class CicekSepetiPriceAndStockUpdateDto
    {
        [JsonPropertyName("stockCode")]
        public string StockCode { get; set; }
        [JsonPropertyName("listPrice")]
        public decimal? ListPrice { get; set; }
        [JsonPropertyName("salesPrice")]
        public decimal? SalesPrice { get; set; }
        [JsonPropertyName("stockQuantity")]
        public int? StockQuantity { get; set; }
    }

    public class CicekSepetiPriceAndStockUpdateRequest
    {
        [JsonPropertyName("items")]
        public List<CicekSepetiPriceAndStockUpdateDto> Items { get; set; } = new List<CicekSepetiPriceAndStockUpdateDto>();
    }
}
