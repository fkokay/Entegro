using Entegro.Application.DTOs.IntegrationSystem;
using Entegro.Application.DTOs.Order;
using Entegro.Application.DTOs.ShipmentItem;

namespace Entegro.Application.DTOs.Shipment
{
    public class CreateShipmentDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public OrderDto Order { get; set; }
        public int? ShippingIntegrationId { get; set; }
        public virtual IntegrationSystemDto? ShippingIntegration { get; set; }
        public string? PrintData { get; set; }
        public bool IsPaymentDoor { get; set; }
        public bool PaymentType { get; set; }
        public string Carrier { get; set; } = string.Empty;
        public string PackageNo { get; set; }
        public string? TrackingNumber { get; set; }
        public string? TrackingUrl { get; set; }
        public decimal? TotalWeight { get; set; }
        public DateTime? ShippedDateUtc { get; set; }
        public DateTime? DeliveryDateUtc { get; set; }
        public DateTime CreatedOn { get; set; }

        public List<CreateShipmentItemDto> ShipmentItems { get; set; } = new List<CreateShipmentItemDto>();
    }
}
