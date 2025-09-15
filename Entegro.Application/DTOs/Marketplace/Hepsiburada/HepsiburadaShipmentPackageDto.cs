using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.Hepsiburada
{
    public class HepsiburadaShipmentPackageDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("customerId")]
        public string CustomerId { get; set; }

        [JsonProperty("orderDate")]
        public DateTime OrderDate { get; set; }

        [JsonProperty("dueDate")]
        public DateTime DueDate { get; set; }

        [JsonProperty("barcode")]
        public string Barcode { get; set; }

        [JsonProperty("packageNumber")]
        public string PackageNumber { get; set; }

        [JsonProperty("cargoCompany")]
        public string CargoCompany { get; set; }

        [JsonProperty("shippingAddressDetail")]
        public string ShippingAddressDetail { get; set; }

        [JsonProperty("recipientName")]
        public string RecipientName { get; set; }

        [JsonProperty("shippingCountryCode")]
        public string ShippingCountryCode { get; set; }

        [JsonProperty("shippingDistrict")]
        public string ShippingDistrict { get; set; }

        [JsonProperty("shippingTown")]
        public string ShippingTown { get; set; }

        [JsonProperty("shippingCity")]
        public string ShippingCity { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("companyName")]
        public string CompanyName { get; set; }

        [JsonProperty("billingAddress")]
        public string BillingAddress { get; set; }

        [JsonProperty("billingCity")]
        public string BillingCity { get; set; }

        [JsonProperty("billingTown")]
        public string BillingTown { get; set; }

        [JsonProperty("billingDistrict")]
        public string BillingDistrict { get; set; }

        [JsonProperty("billingPostalCode")]
        public string BillingPostalCode { get; set; }

        [JsonProperty("billingCountryCode")]
        public string BillingCountryCode { get; set; }

        [JsonProperty("taxOffice")]
        public string TaxOffice { get; set; }

        [JsonProperty("taxNumber")]
        public string TaxNumber { get; set; }

        [JsonProperty("identityNo")]
        public string IdentityNo { get; set; }

        [JsonProperty("shippingTotalPrice")]
        public object ShippingTotalPrice { get; set; }

        [JsonProperty("customsTotalPrice")]
        public object CustomsTotalPrice { get; set; }

        [JsonProperty("totalPrice")]
        public HepsiburadaTotalPrice TotalPrice { get; set; }

        [JsonProperty("items")]
        public List<HepsiburadaItem> Items { get; set; }

        [JsonProperty("isCargoChangable")]
        public bool IsCargoChangable { get; set; }

        [JsonProperty("customerName")]
        public string CustomerName { get; set; }

        [JsonProperty("estimatedArrivalDate")]
        public DateTime EstimatedArrivalDate { get; set; }
    }

    public class HepsiburadaBnplCommissionAmount
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }
    }

    public class HepsiburadaCommission
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }
    }

    public class HepsiburadaItem
    {
        [JsonProperty("lineItemId")]
        public string LineItemId { get; set; }

        [JsonProperty("listingId")]
        public string ListingId { get; set; }

        [JsonProperty("merchantId")]
        public string MerchantId { get; set; }

        [JsonProperty("hbSku")]
        public string HbSku { get; set; }

        [JsonProperty("merchantSku")]
        public string MerchantSku { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("price")]
        public HepsiburadaPrice Price { get; set; }

        [JsonProperty("vat")]
        public int Vat { get; set; }

        [JsonProperty("totalPrice")]
        public HepsiburadaTotalPrice TotalPrice { get; set; }

        [JsonProperty("commission")]
        public HepsiburadaCommission Commission { get; set; }

        [JsonProperty("commissionRate")]
        public int CommissionRate { get; set; }

        [JsonProperty("unitHBDiscount")]
        public HepsiburadaUnitHBDiscount UnitHBDiscount { get; set; }

        [JsonProperty("totalHBDiscount")]
        public HepsiburadaTotalHBDiscount TotalHBDiscount { get; set; }

        [JsonProperty("unitMerchantDiscount")]
        public HepsiburadaUnitMerchantDiscount UnitMerchantDiscount { get; set; }

        [JsonProperty("totalMerchantDiscount")]
        public HepsiburadaTotalMerchantDiscount TotalMerchantDiscount { get; set; }

        [JsonProperty("merchantUnitPrice")]
        public HepsiburadaMerchantUnitPrice MerchantUnitPrice { get; set; }

        [JsonProperty("merchantTotalPrice")]
        public HepsiburadaMerchantTotalPrice MerchantTotalPrice { get; set; }

        [JsonProperty("unitLaborCost")]
        public HepsiburadaUnitLaborCost UnitLaborCost { get; set; }

        [JsonProperty("cargoPaymentInfo")]
        public string CargoPaymentInfo { get; set; }

        [JsonProperty("customizedText01")]
        public string CustomizedText01 { get; set; }

        [JsonProperty("customizedText02")]
        public string CustomizedText02 { get; set; }

        [JsonProperty("customizedText03")]
        public string CustomizedText03 { get; set; }

        [JsonProperty("customizedText04")]
        public string CustomizedText04 { get; set; }

        [JsonProperty("properties")]
        public List<object> Properties { get; set; }

        [JsonProperty("productName")]
        public string ProductName { get; set; }

        [JsonProperty("orderNumber")]
        public string OrderNumber { get; set; }

        [JsonProperty("orderDate")]
        public DateTime OrderDate { get; set; }

        [JsonProperty("deliveryType")]
        public string DeliveryType { get; set; }

        [JsonProperty("customerDelivery")]
        public string CustomerDelivery { get; set; }

        [JsonProperty("pickupTime")]
        public object PickupTime { get; set; }

        [JsonProperty("gtip")]
        public string Gtip { get; set; }

        [JsonProperty("weight")]
        public int Weight { get; set; }

        [JsonProperty("vatRate")]
        public int VatRate { get; set; }

        [JsonProperty("warehouse")]
        public HepsiburadaWarehouse Warehouse { get; set; }

        [JsonProperty("deptorDifferenceAmount")]
        public int DeptorDifferenceAmount { get; set; }

        [JsonProperty("purchasePrice")]
        public HepsiburadaPurchasePrice PurchasePrice { get; set; }

        [JsonProperty("discountToBeBilledToHB")]
        public int DiscountToBeBilledToHB { get; set; }

        [JsonProperty("productBarcode")]
        public string ProductBarcode { get; set; }

        [JsonProperty("bnplCommissionAmount")]
        public HepsiburadaBnplCommissionAmount BnplCommissionAmount { get; set; }

        [JsonProperty("creationReason")]
        public string CreationReason { get; set; }

        [JsonProperty("isMicroExport")]
        public bool IsMicroExport { get; set; }

        [JsonProperty("releatedLineIndexesWithCampaign")]
        public object ReleatedLineIndexesWithCampaign { get; set; }

        [JsonProperty("parentItemIndex")]
        public int ParentItemIndex { get; set; }
    }

    public class HepsiburadaMerchantTotalPrice
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }
    }

    public class HepsiburadaMerchantUnitPrice
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }

    public class HepsiburadaPrice
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }

    public class HepsiburadaPurchasePrice
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }

    public class HepsiburadaTotalHBDiscount
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }

    public class HepsiburadaTotalMerchantDiscount
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }

    public class HepsiburadaTotalPrice
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }

    public class HepsiburadaUnitHBDiscount
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }

    public class HepsiburadaUnitLaborCost
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }

    public class HepsiburadaUnitMerchantDiscount
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }

    public class HepsiburadaWarehouse
    {
        [JsonProperty("shippingModel")]
        public string ShippingModel { get; set; }

        [JsonProperty("shippingAddressLabel")]
        public string ShippingAddressLabel { get; set; }
    }
}
