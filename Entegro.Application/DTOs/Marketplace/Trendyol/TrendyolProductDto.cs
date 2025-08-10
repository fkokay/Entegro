using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.Trendyol
{
    public class TrendyolProductDto
    {
        public string id { get; set; }
        public bool approved { get; set; }
        public bool archived { get; set; }
        public long productCode { get; set; }
        public string batchRequestId { get; set; }
        public long supplierId { get; set; }
        public long createDateTime { get; set; } // Unix timestamp
        public long lastUpdateDate { get; set; } // Unix timestamp
        public string gender { get; set; }
        public string brand { get; set; }
        public string barcode { get; set; }
        public string title { get; set; }
        public string categoryName { get; set; }
        public string productMainId { get; set; }
        public string description { get; set; }
        public string stockUnitType { get; set; }
        public int quantity { get; set; }
        public decimal listPrice { get; set; }
        public decimal salePrice { get; set; }
        public int vatRate { get; set; }
        public decimal dimensionalWeight { get; set; }
        public string stockCode { get; set; }
        public TrendyolDeliveryOptionDto deliveryOption { get; set; }
        public List<TrendyolProductImageDto> images { get; set; }
        public List<TrendyolProductAttributeDto> attributes { get; set; }
        public string platformListingId { get; set; }
        public string stockId { get; set; }
        public bool hasActiveCampaign { get; set; }
        public bool locked { get; set; }
        public long productContentId { get; set; }
        public int pimCategoryId { get; set; }
        public long brandId { get; set; }
        public int version { get; set; }
        public string color { get; set; }
        public string size { get; set; }
        public bool lockedByUnSuppliedReason { get; set; }
        public bool onsale { get; set; }
        public string productUrl { get; set; }
    }
}
