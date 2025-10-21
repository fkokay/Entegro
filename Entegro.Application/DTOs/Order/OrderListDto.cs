using Entegro.Application.DTOs.IntegrationSystem;
using Entegro.Application.DTOs.OrderItem;

namespace Entegro.Application.DTOs.Order
{
    public class OrderListDto
    {
        public int Id { get; set; }
        public string PackageNo { get; set; }
        public int? IntegrationSystemId { get; set; }
        public IntegrationSystemDto? IntegrationSystem { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DueDate { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int CustomerOrderCounts { get; set; }
        public string ShipmentCarrier { get; set; }
        public string ShippingTrackingNumber { get; set; }
        public string TrackingUrl { get; set; }
        public DateTime ShippedDateUtc { get; set; }
        public DateTime DeliveryDateUtc { get; set; }
        public int OrderStatusId { get; set; }
        public int ShippingStatusId { get; set; }
        public decimal OrderSubTotal { get; set; }
        public decimal OrderDiscount { get; set; }
        public decimal OrderTotal { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public List<OrderItemListDto> OrderItems { get; set; } = new List<OrderItemListDto>();
    }
}
