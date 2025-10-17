namespace Entegro.Application.DTOs.ShipmentItem
{
    public class UpdateShipmentItemDto
    {
        public int Id { get; set; }
        public int ShipmentId { get; set; }
        public int OrderItemId { get; set; }
        public int Quantity { get; set; }

    }
}
