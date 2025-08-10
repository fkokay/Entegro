using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Commerce.Smartstore
{
    public class SmartstoreOrderItemDto
    {
        public Guid OrderItemGuid { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string Sku { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPriceInclTax { get; set; }
        public decimal UnitPriceExclTax { get; set; }
        public decimal PriceInclTax { get; set; }
        public decimal PriceExclTax { get; set; }
        public decimal TaxRate { get; set; }
        public decimal DiscountAmountInclTax { get; set; }
        public decimal DiscountAmountExclTax { get; set; }
        public string AttributeDescription { get; set; }
        public string RawAttributes { get; set; }
        public int DownloadCount { get; set; }
        public bool IsDownloadActivated { get; set; }
        public int LicenseDownloadId { get; set; }
        public decimal ItemWeight { get; set; }
        public string BundleData { get; set; }
        public decimal ProductCost { get; set; }
        public int? DeliveryTimeId { get; set; }
        public bool DisplayDeliveryTime { get; set; }
        public int Id { get; set; }

        public SmartstoreProductDto Product { get; set; }
    }
}
