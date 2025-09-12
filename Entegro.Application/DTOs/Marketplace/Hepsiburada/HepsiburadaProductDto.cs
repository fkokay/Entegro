using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.Hepsiburada
{
    public class HepsiburadaProductDto
    {
        [JsonProperty("listingId")]
        public string ListingId { get; set; }

        [JsonProperty("uniqueIdentifier")]
        public string UniqueIdentifier { get; set; }

        [JsonProperty("hepsiburadaSku")]
        public string HepsiburadaSku { get; set; }

        [JsonProperty("merchantSku")]
        public string MerchantSku { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("availableStock")]
        public int AvailableStock { get; set; }

        [JsonProperty("dispatchTime")]
        public int DispatchTime { get; set; }

        [JsonProperty("cargoCompany1")]
        public string CargoCompany1 { get; set; }

        [JsonProperty("cargoCompany2")]
        public string CargoCompany2 { get; set; }

        [JsonProperty("cargoCompany3")]
        public string CargoCompany3 { get; set; }

        [JsonProperty("shippingAddressLabel")]
        public string ShippingAddressLabel { get; set; }

        [JsonProperty("shippingProfileName")]
        public string ShippingProfileName { get; set; }

        [JsonProperty("claimAddressLabel")]
        public string ClaimAddressLabel { get; set; }

        [JsonProperty("maximumPurchasableQuantity")]
        public int MaximumPurchasableQuantity { get; set; }

        [JsonProperty("minimumPurchasableQuantity")]
        public int MinimumPurchasableQuantity { get; set; }

        [JsonProperty("pricings")]
        public List<object> Pricings { get; set; }

        [JsonProperty("isSalable")]
        public bool IsSalable { get; set; }

        [JsonProperty("customizableProperties")]
        public List<object> CustomizableProperties { get; set; }

        [JsonProperty("deactivationReasons")]
        public List<object> DeactivationReasons { get; set; }

        [JsonProperty("isSuspended")]
        public bool IsSuspended { get; set; }

        [JsonProperty("isLocked")]
        public bool IsLocked { get; set; }

        [JsonProperty("lockReasons")]
        public List<object> LockReasons { get; set; }

        [JsonProperty("isFrozen")]
        public bool IsFrozen { get; set; }

        [JsonProperty("freezeReasons")]
        public List<object> FreezeReasons { get; set; }

        [JsonProperty("availableWarehouses")]
        public List<object> AvailableWarehouses { get; set; }

        [JsonProperty("isFulfilledByHB")]
        public bool IsFulfilledByHB { get; set; }

        [JsonProperty("priceIncreaseDisabled")]
        public bool PriceIncreaseDisabled { get; set; }

        [JsonProperty("priceDecreaseDisabled")]
        public bool PriceDecreaseDisabled { get; set; }

        [JsonProperty("stockDecreaseDisabled")]
        public bool StockDecreaseDisabled { get; set; }

        [JsonProperty("skuAfterSuspension")]
        public object SkuAfterSuspension { get; set; }

        [JsonProperty("productId")]
        public string ProductId { get; set; }

        [JsonProperty("hasVariant")]
        public bool HasVariant { get; set; }
    }
}
