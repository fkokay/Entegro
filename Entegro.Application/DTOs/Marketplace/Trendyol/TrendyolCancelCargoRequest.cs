namespace Entegro.Application.DTOs.Marketplace.Trendyol
{
    public class TrendyolCancelCargoRequest
    {
        public string ShipmentPackageNumber { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;// OutOfStock, CustomerRequest, AddressIssue vb.
    }

}
