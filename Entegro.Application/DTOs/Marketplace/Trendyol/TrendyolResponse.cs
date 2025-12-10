namespace Entegro.Application.DTOs.Marketplace.Trendyol
{
    public class TrendyolResponse<T>
    {
        public int totalElements { get; set; }
        public int totalPages { get; set; }
        public int page { get; set; }
        public int size { get; set; }
        public List<T> content { get; set; }
    }

    public class Content
    {
        public string id { get; set; }
        public string claimId { get; set; }
        public string orderNumber { get; set; }
        public long orderDate { get; set; }
        public string customerFirstName { get; set; }
        public string customerLastName { get; set; }
        public long claimDate { get; set; }
        public long cargoTrackingNumber { get; set; }
        public string cargoProviderName { get; set; }
        public string cargoTrackingLink { get; set; }
        public long orderShipmentPackageId { get; set; }
        public Item[] items { get; set; }
        public long lastModifiedDate { get; set; }
        public long orderOutboundPackageId { get; set; }
    }

    public class Item
    {
        public Orderline orderLine { get; set; }
        public Claimitem[] claimItems { get; set; }
    }

    public class Orderline
    {
        public long id { get; set; }
        public string productName { get; set; }
        public string barcode { get; set; }
        public string merchantSku { get; set; }
        public string productColor { get; set; }
        public string productSize { get; set; }
        public float price { get; set; }
        public int vatBaseAmount { get; set; }
        public int vatRate { get; set; }
        public int salesCampaignId { get; set; }
        public string productCategory { get; set; }
        public object lineItems { get; set; }
    }

    public class Claimitem
    {
        public string id { get; set; }
        public long orderLineItemId { get; set; }
        public Customerclaimitemreason customerClaimItemReason { get; set; }
        public Trendyolclaimitemreason trendyolClaimItemReason { get; set; }
        public Claimitemstatus claimItemStatus { get; set; }
        public long autoApproveDate { get; set; }
        public string note { get; set; }
        public string customerNote { get; set; }
        public bool resolved { get; set; }
        public object autoAccepted { get; set; }
        public bool? acceptedBySeller { get; set; }

    }

    public class Customerclaimitemreason
    {
        public int id { get; set; }
        public string name { get; set; }
        public int externalReasonId { get; set; }
        public string code { get; set; }
    }

    public class Trendyolclaimitemreason
    {
        public int id { get; set; }
        public string name { get; set; }
        public int externalReasonId { get; set; }
        public string code { get; set; }
    }

    public class Claimitemstatus
    {
        public string name { get; set; }
    }


}
