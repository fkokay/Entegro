namespace Entegro.Application.DTOs.Order
{
    public class OrderListPageDto
    {
        public int ToBePackedQuantity { get; set; }
        public int ReadyToShipQuantity { get; set; }
        public int ShippedQuantity { get; set; }
        public int DeliveredQuantity { get; set; }
        public int UnDeliverdQuantity { get; set; }
        public int PaymentAwaitingQuantity { get; set; }
        public int CancalledQuantity { get; set; }
    }
}
