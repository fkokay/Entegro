using Entegro.Domain.Entities.Common;
using Entegro.Domain.Entities.Integration;
using Entegro.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Domain.Entities.Checkout
{
    public class OrderMap : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasQueryFilter(c => !c.Deleted);
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
        public string OrderNumber { get; set; }
        public Guid OrderGuid { get; set; }
        public int? IntegrationSystemId { get; set; }
        public virtual IntegrationSystem? IntegrationSystem { get; set; }
        public string? IntegrationOrderNumber { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        public int? BillingAddressId { get; set; }
        public virtual Address? BillingAddress { get; set; }
        public int? ShippingAddressId { get; set; }
        public virtual Address? ShippingAddress { get; set; }
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
        public string? InvoiceLink { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
        public virtual ICollection<OrderNote> OrderNotes { get; set; } = new HashSet<OrderNote>();
        public virtual ICollection<Shipment> Shipments { get; set; } = new HashSet<Shipment>();
    }
}
