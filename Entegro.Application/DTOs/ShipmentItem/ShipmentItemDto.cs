using Entegro.Application.DTOs.Shipment;

namespace Entegro.Application.DTOs.ShipmentItem
{
    public class ShipmentItemDto
    {
        public int Id { get; set; }
        public int ShipmentId { get; set; }
        public virtual ShipmentDto Shipment { get; set; }
        public int OrderItemId { get; set; }
        public int Quantity { get; set; }
    }
}
