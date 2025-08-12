using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.Trendyol
{
    public class TrendyolOrderLineDto
    {
        public int Quantity { get; set; }
        public long SalesCampaignId { get; set; }
        public string ProductSize { get; set; }
        public string MerchantSku { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string ProductOrigin { get; set; }
        public long MerchantId { get; set; }
        public decimal Amount { get; set; }
        public decimal Discount { get; set; }
        public decimal TyDiscount { get; set; }
        public List<TrendyolDiscountDetailDto> DiscountDetails { get; set; }
        public List<TrendyolFastDeliveryOptionDto> FastDeliveryOptions { get; set; }
        public string CurrencyCode { get; set; }
        public string ProductColor { get; set; }
        public long Id { get; set; } // orderLineId
        public string Sku { get; set; }
        public decimal VatBaseAmount { get; set; }
        public string Barcode { get; set; }
        public string OrderLineItemStatusName { get; set; }
        public decimal Price { get; set; }
        public long ProductCategoryId { get; set; }
        public decimal LaborCost { get; set; }
    }
}
