using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.CicekSepeti
{
    public class CicekSepetiProductDto
    {
        [JsonProperty("productName")]
        public string ProductName { get; set; }

        [JsonProperty("productCode")]
        public string ProductCode { get; set; }

        [JsonProperty("stockCode")]
        public string StockCode { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("categoryId")]
        public int CategoryId { get; set; }

        [JsonProperty("categoryName")]
        public string CategoryName { get; set; }

        [JsonProperty("mainProductCode")]
        public string MainProductCode { get; set; }

        [JsonProperty("productStatusType")]
        public string ProductStatusType { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("deliveryMessageType")]
        public int DeliveryMessageType { get; set; }

        [JsonProperty("deliveryType")]
        public int DeliveryType { get; set; }

        [JsonProperty("isUseStockQuantity")]
        public bool IsUseStockQuantity { get; set; }

        [JsonProperty("stockQuantity")]
        public int StockQuantity { get; set; }

        [JsonProperty("salesPrice")]
        public double SalesPrice { get; set; }

        [JsonProperty("listPrice")]
        public double ListPrice { get; set; }

        [JsonProperty("barcode")]
        public string Barcode { get; set; }

        [JsonProperty("commissionRate")]
        public string CommissionRate { get; set; }

        [JsonProperty("numberOfFavorites")]
        public int NumberOfFavorites { get; set; }

        [JsonProperty("variantName")]
        public string VariantName { get; set; }

        [JsonProperty("passiveDescription")]
        public string PassiveDescription { get; set; }

        [JsonProperty("images")]
        public List<string> Images { get; set; }

        [JsonProperty("attributes")]
        public List<CiceckSepetiAttributeDto> Attributes { get; set; } = new List<CiceckSepetiAttributeDto>();

        [JsonProperty("operatorContacts")]
        public List<CicekSepetiOperatorContactDto> OperatorContacts { get; set; } = new List<CicekSepetiOperatorContactDto>();

        [JsonProperty("safetyInfo")]
        public CicekSepetiSafetyInfoDto? SafetyInfo { get; set; }
    }
}
