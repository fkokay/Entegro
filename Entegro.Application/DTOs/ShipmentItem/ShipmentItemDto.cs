using Entegro.Application.DTOs.OrderItem;
using Entegro.Application.DTOs.Shipment;

namespace Entegro.Application.DTOs.ShipmentItem
{
    public class ShipmentItemDto
    {
        public int Id { get; set; }
        public int ShipmentId { get; set; }
        public int OrderItemId { get; set; }
        public int Quantity { get; set; }

        public OrderItemDto OrderItem { get; set; }
    }
}
