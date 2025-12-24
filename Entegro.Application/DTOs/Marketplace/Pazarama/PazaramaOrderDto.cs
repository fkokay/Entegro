namespace Entegro.Application.DTOs.Marketplace.Pazarama
{
    public class PazaramaOrderDto
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public List<OrderData> Data { get; set; }
        public bool Success { get; set; }
        public string MessageCode { get; set; }
        public string Message { get; set; }
        public string UserMessage { get; set; }
        public bool FromCache { get; set; }
    }
    public class OrderData
    {
        public string OrderId { get; set; }
        public long OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public double OrderAmount { get; set; }
        public double ShipmentAmount { get; set; }
        public double DiscountAmount { get; set; }
        public string DiscountDescription { get; set; }
        public string Currency { get; set; }
        public int PaymentType { get; set; }
        public int OrderStatus { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public ShipmentAddress ShipmentAddress { get; set; }
        public BillingAddress BillingAddress { get; set; }
        public List<OrderItem> Items { get; set; }
    }
    public class ShipmentAddress
    {
        public string AddressId { get; set; }
        public string Title { get; set; }
        public string NameSurname { get; set; }
        public string CustomerEmail { get; set; }
        public string CityName { get; set; }
        public string DistrictName { get; set; }
        public string NeighborhoodName { get; set; }
        public string AddressDetail { get; set; }
        public string DisplayAddressText { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class BillingAddress
    {
        public string AddressId { get; set; }
        public string Title { get; set; }
        public string NameSurname { get; set; }
        public string CustomerEmail { get; set; }
        public string CityName { get; set; }
        public string DistrictName { get; set; }
        public string NeighborhoodName { get; set; }
        public string AddressDetail { get; set; }
        public string DisplayAddressText { get; set; }
        public string PhoneNumber { get; set; }
        public string IdentityNumber { get; set; }
        public bool IsInvoiceObliged { get; set; }
    }
    public class OrderItem
    {
        public string OrderItemId { get; set; }
        public int OrderItemStatus { get; set; }
        public string OrderItemStatusName { get; set; }
        public string ShipmentCode { get; set; }
        public Price ShipmentCost { get; set; }
        public Price ListPrice { get; set; }
        public Price SalePrice { get; set; }
        public Price TaxAmount { get; set; }
        public Price ShipmentAmount { get; set; }
        public Price TotalPrice { get; set; }
        public Price DiscountAmount { get; set; }
        public Cargo Cargo { get; set; }
        public Product Product { get; set; }
        public Price LaborCostItem { get; set; }
        public bool PlusOrder { get; set; }
        public int ChannelCode { get; set; }
        public string ChannelCodeName { get; set; }
        public int Quantity { get; set; }
    }
    public class Price
    {
        public string Currency { get; set; }
        public double Value { get; set; }
        public long ValueInt { get; set; }
        public string ValueString { get; set; }
        public string WholePartString { get; set; }
        public string FractionPartString { get; set; }
        public string DecimalSeparator { get; set; }
    }
    public class Cargo
    {
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string TrackingNumber { get; set; }
        public string TrackingUrl { get; set; }
    }

    public class Product
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string VariantOptionDisplay { get; set; }
        public string StockCode { get; set; }
        public string Barcode { get; set; }
        public int VatRate { get; set; }
        public string SellerAddressId { get; set; }
        public string Code { get; set; }
        public string? Url { get; set; }
    }

}
