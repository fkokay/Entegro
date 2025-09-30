using Entegro.Application.DTOs.ShipmentItem;

namespace Entegro.Application.DTOs.Shipment
{
    public class ShipmentDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Carrier { get; set; } = string.Empty;
        public string PackageNo { get; set; }
        public string? TrackingNumber { get; set; }
        public string? TrackingUrl { get; set; }
        public decimal? TotalWeight { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public virtual ICollection<ShipmentItemDto> ShipmentItems { get; set; } = new HashSet<ShipmentItemDto>();
    }
}
