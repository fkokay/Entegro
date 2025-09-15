namespace Entegro.Application.DTOs.ShipmentItem
{
    public class CreateShipmentItemDto
    {
        public int ShipmentId { get; set; }
        public int OrderItemId { get; set; }
        public int Quantity { get; set; }
    }
}
