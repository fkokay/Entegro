using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.N11
{
    public class N11OrderLineDto
    {
        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("productId")]
        public int ProductId { get; set; }

        [JsonProperty("productName")]
        public string ProductName { get; set; }

        [JsonProperty("stockCode")]
        public string StockCode { get; set; }

        [JsonProperty("variantAttributes")]
        public List<N11VariantAttributeDto> VariantAttributes { get; set; }

        [JsonProperty("customTextOptionValues")]
        public List<object> CustomTextOptionValues { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("dueAmount")]
        public double DueAmount { get; set; }

        [JsonProperty("installmentChargeWithVAT")]
        public int InstallmentChargeWithVAT { get; set; }

        [JsonProperty("sellerCouponDiscount")]
        public int SellerCouponDiscount { get; set; }

        [JsonProperty("sellerCampaignCommissionDiscount")]
        public int SellerCampaignCommissionDiscount { get; set; }

        [JsonProperty("sellerDiscount")]
        public double SellerDiscount { get; set; }

        [JsonProperty("mallDiscount")]
        public double MallDiscount { get; set; }

        [JsonProperty("sellerInvoiceAmount")]
        public double SellerInvoiceAmount { get; set; }

        [JsonProperty("totalMallDiscountPrice")]
        public double TotalMallDiscountPrice { get; set; }

        [JsonProperty("orderLineId")]
        public int OrderLineId { get; set; }

        [JsonProperty("orderItemLineItemStatusName")]
        public string OrderItemLineItemStatusName { get; set; }

        [JsonProperty("totalSellerDiscountPrice")]
        public double TotalSellerDiscountPrice { get; set; }

        [JsonProperty("vatRate")]
        public int VatRate { get; set; }

        [JsonProperty("commissionRate")]
        public int CommissionRate { get; set; }

        [JsonProperty("sellerCampaignCommissionRate")]
        public int SellerCampaignCommissionRate { get; set; }

        [JsonProperty("taxDeductionRate")]
        public int TaxDeductionRate { get; set; }

        [JsonProperty("totalLaborCostExcludingVAT")]
        public int TotalLaborCostExcludingVAT { get; set; }

        [JsonProperty("netMarketingFeeRate")]
        public double NetMarketingFeeRate { get; set; }

        [JsonProperty("netMarketplaceFeeRate")]
        public double NetMarketplaceFeeRate { get; set; }

        [JsonProperty("barcode")]
        public string Barcode { get; set; }
    }
}
