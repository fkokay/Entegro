using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.N11
{
    public class N11PriceAndStockUpdateDto
    {
        [JsonPropertyName("stockCode")]
        public string StockCode { get; set; }
        [JsonPropertyName("listPrice")]
        public decimal ListPrice { get; set; }
        [JsonPropertyName("salePrice")]
        public decimal SalePrice { get; set; }
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
        [JsonPropertyName("currencyType")]
        public string CurrencyType { get; set; }
    }

    public class N11PriceAndStockUpdateRequest
    {
        [JsonPropertyName("integrator")]
        public string Integrator { get; set; }
        [JsonPropertyName("skus")]
        public List<N11PriceAndStockUpdateDto> Skus { get; set; }
    }

    public class N11PriceAndStockUpdatePayload
    {
        [JsonPropertyName("payload")]
        public N11PriceAndStockUpdateRequest Payload { get; set; }
    }
}
