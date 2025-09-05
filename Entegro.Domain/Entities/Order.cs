
using Entegro.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Domain.Entities
{
    public class OrderMap : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasQueryFilter(c => !c.Deleted);

            builder.Property(x => x.CurrencyRate).HasPrecision(18, 8);
            builder.Property(x => x.OrderTotal).HasPrecision(18, 4);

            builder
               .HasOne(x => x.Customer)
               .WithMany(x => x.Orders)
               .HasForeignKey(x => x.CustomerId);

            builder
                .HasOne(o => o.BillingAddress)
                .WithMany()
                .HasForeignKey(o => o.BillingAddressId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder
                .HasOne(o => o.ShippingAddress)
                .WithMany()
                .HasForeignKey(o => o.ShippingAddressId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }

    [Table("Order")]
    public class Order : BaseEntity, ISoftDeletable, ITransient
    {
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
                    _ => throw new NotImplementedException(),
                };
            }
        }
        public string OrderNumber { get; set; }
        public Guid OrderGuid { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }
        public int? BillingAddressId { get; set; }
        public virtual Address? BillingAddress { get; set; }
        public int? ShippingAddressId { get; set; }
        public virtual Address? ShippingAddress { get; set; }
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

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
        public virtual ICollection<OrderNote> OrderNotes { get; set; } = new HashSet<OrderNote>();
        public virtual ICollection<Shipment> Shipments { get; set; } = new HashSet<Shipment>();
    }
}
