using Entegro.Domain.Entities.Checkout;
using Entegro.Domain.Entities.Integration;
using Entegro.Domain.Enums;
using Entegro.Web.Models.Common;
using Entegro.Web.Models.Integration.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Web.Models.Checkout.Orders
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public Guid OrderGuid { get; set; }
        public int? IntegrationSystemId { get; set; }
        public virtual IntegrationSystemViewModel? IntegrationSystem { get; set; }
        public string? IntegrationOrderNumber { get; set; }
        public int CustomerId { get; set; }
        public virtual CustomerViewModel Customer { get; set; }
        public int? BillingAddressId { get; set; }
        public virtual AddressViewModel? BillingAddress { get; set; }
        public int? ShippingAddressId { get; set; }
        public virtual AddressViewModel? ShippingAddress { get; set; }
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

        [NotMapped]
        public string PaymentStatusLabelHint
        {
            get
            {
                return PaymentStatus switch
                {
                    PaymentStatus.Pending => "Beklemede",
                    PaymentStatus.Authorized => "Onaylandı",
                    PaymentStatus.Paid => "Ödendi",
                    PaymentStatus.PartiallyRefunded => "Kısmen İade Edildi",
                    PaymentStatus.Refunded => "İade Edildi",
                    PaymentStatus.Voided => "Geçersiz",
                    _ => throw new NotImplementedException(),
                };
            }
        }

        public int ShippingStatusId { get; set; }

        [NotMapped]
        public ShippingStatus ShippingStatus
        {
            get => (ShippingStatus)ShippingStatusId;
            set => ShippingStatusId = (int)value;
        }
        public virtual List<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();
        public virtual List<OrderNoteViewModel> OrderNotes { get; set; } = new List<OrderNoteViewModel>();
    }
}
