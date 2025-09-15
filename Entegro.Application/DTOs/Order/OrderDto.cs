using Entegro.Application.DTOs.Address;
using Entegro.Application.DTOs.Customer;
using Entegro.Application.DTOs.OrderItem;
using Entegro.Application.DTOs.OrderNote;
using Entegro.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Application.DTOs.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int OrderSourceId { get; set; }
        [NotMapped]
        public OrderSource OrderSource
        {
            get => (OrderSource)OrderSourceId;
            set => OrderSourceId = (int)value;
        }
        [NotMapped]
        public string OrderSourceLabelHint
        {
            get
            {
                return OrderSource switch
                {
                    OrderSource.Smartstore => "Smartstore",
                    OrderSource.Trendyol => "Trendyol",
                    OrderSource.Hepsiburada => "Hepsiburada",
                    _ => throw new NotImplementedException(),
                };
            }
        }
        public string OrderNumber { get; set; }
        public Guid OrderGuid { get; set; }
        public int CustomerId { get; set; }
        public virtual CustomerDto Customer { get; set; }
        public int? BillingAddressId { get; set; }
        public virtual AddressDto? BillingAddress { get; set; }
        public int? ShippingAddressId { get; set; }
        public virtual AddressDto? ShippingAddress { get; set; }
        public string PaymentMethodSystemName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal CurrencyRate { get; set; }
        public string VatNumber { get; set; }
        public decimal OrderSubtotalInclTax { get; set; }
        public decimal OrderSubtotalExclTax { get; set; }
        public decimal OrderSubTotalDiscountInclTax { get; set; }
        public decimal OrderSubTotalDiscountExclTax { get; set; }
        public decimal OrderShippingInclTax { get; set; }
        public decimal OrderShippingExclTax { get; set; }
        public decimal OrderShippingTaxRate { get; set; }
        public decimal PaymentMethodAdditionalFeeInclTax { get; set; }
        public decimal PaymentMethodAdditionalFeeExclTax { get; set; }
        public decimal PaymentMethodAdditionalFeeTaxRate { get; set; }
        public decimal OrderTax { get; set; }
        public decimal OrderDiscount { get; set; }
        public decimal OrderTotal { get; set; }
        public decimal RefundedAmount { get; set; }
        public string? CustomerIp { get; set; }
        public bool Deleted { get; set; }
        public bool IsTransient { get; set; }
        public string TaxRates { get; set; }
        public DateTime? PaidDateUtc { get; set; }
        public string ShippingMethod { get; set; }
        public int OrderStatusId { get; set; }

        [NotMapped]
        public OrderStatus OrderStatus
        {
            get => (OrderStatus)OrderStatusId;
            set => OrderStatusId = (int)value;
        }

        [NotMapped]
        public string OrderStatusLabelHint
        {
            get
            {
                return OrderStatus switch
                {
                    OrderStatus.Pending => "Beklemede",
                    OrderStatus.Processing => "Hazırlanıyor",
                    OrderStatus.Complete => "Tamamlandı",
                    OrderStatus.Cancelled => "İptal edildi",
                    _ => throw new NotImplementedException(),
                };
            }
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

        [NotMapped]
        public string ShippingStatusLabelHint
        {
            get
            {
                return ShippingStatus switch
                {
                    ShippingStatus.ShippingNotRequired => "Nakliye gerekli değil",
                    ShippingStatus.NotYetShipped => "Henüz teslim edilmedi",
                    ShippingStatus.PartiallyShipped => "Kısmen teslim edildi",
                    ShippingStatus.Shipped => "Gönderildi",
                    ShippingStatus.Delivered => "Teslim edildi",
                    _ => throw new NotImplementedException(),
                };
            }
        }

        public decimal CalculateTotalAmount()
        {
            return OrderItems?.Sum(item =>
            {

                var subtotal = item.UnitPrice * item.Quantity;

                return subtotal;
            }) ?? 0;
        }

        public virtual List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
        public virtual List<OrderNoteDto> OrderNotes { get; set; } = new List<OrderNoteDto>();


    }
}
