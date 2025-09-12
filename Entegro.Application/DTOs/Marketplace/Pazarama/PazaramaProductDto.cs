using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.Pazarama
{
    public class PazaramaProductDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("description")]
        public object Description { get; set; }

        [JsonProperty("brandName")]
        public string BrandName { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("groupCode")]
        public string GroupCode { get; set; }

        [JsonProperty("stockCount")]
        public int StockCount { get; set; }

        [JsonProperty("stockCode")]
        public string StockCode { get; set; }

        [JsonProperty("priorityRank")]
        public int PriorityRank { get; set; }

        [JsonProperty("listPrice")]
        public double ListPrice { get; set; }

        [JsonProperty("salePrice")]
        public double SalePrice { get; set; }

        [JsonProperty("vatRate")]
        public int VatRate { get; set; }

        [JsonProperty("categoryName")]
        public string CategoryName { get; set; }

        [JsonProperty("categoryId")]
        public string CategoryId { get; set; }

        [JsonProperty("state")]
        public int State { get; set; }

        [JsonProperty("status")]
        public object Status { get; set; }

        [JsonProperty("waitingApproveExp")]
        public object WaitingApproveExp { get; set; }

        [JsonProperty("productSaleLimitDetail")]
        public PazaramaProductSaleLimitDetailDto ProductSaleLimitDetail { get; set; }

        [JsonProperty("attributes")]
        public object Attributes { get; set; }

        [JsonProperty("images")]
        public object Images { get; set; }

        [JsonProperty("deliveryTypes")]
        public object DeliveryTypes { get; set; }

        [JsonProperty("productStatus")]
        public int ProductStatus { get; set; }
    }
}
