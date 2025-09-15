namespace Entegro.Application.DTOs.Shipment
{
    public class CreateShipmentDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Carrier { get; set; } = string.Empty;
        public string? TrackingNumber { get; set; }
        public string? TrackingUrl { get; set; }
        public decimal? TotalWeight { get; set; }
        public DateTime? ShippedDateUtc { get; set; }
        public DateTime? DeliveryDateUtc { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}
