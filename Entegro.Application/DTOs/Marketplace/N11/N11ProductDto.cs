using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.N11
{
    public class N11ProductDto
    {
        [JsonPropertyName("n11ProductId")]
        public int N11ProductId { get; set; }

        [JsonPropertyName("sellerId")]
        public int SellerId { get; set; }

        [JsonPropertyName("sellerNickname")]
        public string SellerNickname { get; set; }

        [JsonPropertyName("stockCode")]
        public string StockCode { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("categoryId")]
        public int CategoryId { get; set; }

        [JsonPropertyName("productMainId")]
        public int? ProductMainId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("saleStatus")]
        public string SaleStatus { get; set; }

        [JsonPropertyName("preparingDay")]
        public int PreparingDay { get; set; }

        [JsonPropertyName("shipmentTemplate")]
        public string ShipmentTemplate { get; set; }

        [JsonPropertyName("maxPurchaseQuantity")]
        public int? MaxPurchaseQuantity { get; set; }

        [JsonPropertyName("customTextOptions")]
        public List<object> CustomTextOptions { get; set; }

        [JsonPropertyName("catalogId")]
        public int? CatalogId { get; set; }

        [JsonPropertyName("barcode")]
        public string Barcode { get; set; }

        [JsonPropertyName("groupId")]
        public int GroupId { get; set; }

        [JsonPropertyName("currencyType")]
        public string CurrencyType { get; set; }

        [JsonPropertyName("salePrice")]
        public double SalePrice { get; set; }

        [JsonPropertyName("listPrice")]
        public double ListPrice { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("attributes")]
        public List<N11Attribute> Attributes { get; set; }

        [JsonPropertyName("imageUrls")]
        public List<string> ImageUrls { get; set; }

        [JsonPropertyName("vatRate")]
        public int VatRate { get; set; }

        [JsonPropertyName("commissionRate")]
        public double CommissionRate { get; set; }

        [JsonPropertyName("rejectInfo")]
        public string RejectInfo { get; set; }
    }
}
