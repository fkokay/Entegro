using Entegro.Application.DTOs.Address;
using Entegro.Application.DTOs.Customer;
using Entegro.Application.DTOs.IntegrationSystem;
using Entegro.Application.DTOs.OrderItem;
using Entegro.Application.DTOs.OrderNote;
using Entegro.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Application.DTOs.Order
{
    public class CreateOrderDto
    {
        public string OrderNumber { get; set; }
        public Guid OrderGuid { get; set; }
        public int? IntegrationSystemId { get; set; }
        public virtual IntegrationSystemDto? IntegrationSystem { get; set; }
        public string? IntegrationOrderNumber { get; set; }
        public int CustomerId { get; set; }
        public virtual CustomerDto Customer { get; set; }
        public int? BillingAddressId { get; set; }
        public virtual AddressDto? BillingAddress { get; set; }
        public int? ShippingAddressId { get; set; }
        public virtual AddressDto? ShippingAddress { get; set; }
        public string PaymentMethod { get; set; }
        public decimal PaymentFee { get; set; }
        public string ShippingMethod { get; set; }
        public decimal OrderShipping { get; set; }
        public DateTime OrderDateUtc { get; set; }
        public decimal OrderSubTotal { get; set; }
        public decimal OrderDiscount { get; set; }
        public decimal OrderTax { get; set; }
        public decimal OrderTotal { get; set; }
        public decimal RefundedAmount { get; set; }
        public bool Deleted { get; set; }
        public bool IsTransient { get; set; }
        public DateTime? PaidDateUtc { get; set; }
        public DateTime DueDateUtc { get; set; }
        public int OrderStatusId { get; set; }
        public string? InvoiceLink { get; set; }
        [NotMapped]
        public OrderStatus OrderStatus
        {
            get => (OrderStatus)OrderStatusId;
            set => OrderStatusId = (int)value;
        }

        public int PaymentStatusId { get; set; }

        [NotMapped]
        public PaymentStatus PaymentStatus
        {
            get => (PaymentStatus)PaymentStatusId;
            set => PaymentStatusId = (int)value;
        }

        public int ShippingStatusId { get; set; }

        [NotMapped]
        public ShippingStatus ShippingStatus
        {
            get => (ShippingStatus)ShippingStatusId;
            set => ShippingStatusId = (int)value;
        }

        public List<CreateOrderItemDto> OrderItems { get; set; } = new();
        public List<OrderNoteDto> OrderNotes { get; set; } = new();
    }
}
