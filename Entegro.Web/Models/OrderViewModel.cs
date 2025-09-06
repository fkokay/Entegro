using Entegro.Domain.Enums;

namespace Entegro.Web.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public int OrderSourceId { get; set; }
        public OrderSource? OrderSource { get; set; }
        public string OrderSourceLabelHint { get; set; }
        public string OrderNumber { get; set; }
        public Guid OrderGuid { get; set; }
        public int CustomerId { get; set; }
        public CustomerViewModel Customer { get; set; }
        public int? BillingAddressId { get; set; }
        public virtual AddressViewModel? BillingAddress { get; set; }
        public int? ShippingAddressId { get; set; }
        public virtual AddressViewModel? ShippingAddress { get; set; }
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
        public OrderStatus OrderStatus { get; set; }
        public int PaymentStatusId { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public int ShippingStatusId { get; set; }
        public ShippingStatus ShippingStatus { get; set; }
        public decimal CalculateTotalAmount { get; set; }
        public virtual List<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();
        public virtual List<OrderNoteViewModel> OrderNotes { get; set; } = new List<OrderNoteViewModel>();
    }
}
