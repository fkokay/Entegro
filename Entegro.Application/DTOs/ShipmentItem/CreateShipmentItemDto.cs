using Entegro.Application.DTOs.OrderItem;
using Entegro.Application.DTOs.Shipment;

namespace Entegro.Application.DTOs.ShipmentItem
{
    public class CreateShipmentItemDto
    {
        public int ShipmentId { get; set; }
        public ShipmentDto Shipment { get; set; }
        public int OrderItemId { get; set; }
        public OrderItemDto OrderItem { get; set; }
        public int Quantity { get; set; }
    }
}
