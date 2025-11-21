using Entegro.Domain.Entities.Integration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Domain.Entities.Checkout
{
    public class ShipmentMap : IEntityTypeConfiguration<Shipment>
    {
        public void Configure(EntityTypeBuilder<Shipment> builder)
        {
            builder.HasMany(p => p.ShipmentItems)
                  .WithOne(pc => pc.Shipment)
                  .HasForeignKey(pc => pc.ShipmentId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }

    [Table("Shipment")]
    public class Shipment : BaseEntity
    {
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public int? ShippingIntegrationId { get; set; }
        public virtual IntegrationSystem? ShippingIntegration { get; set; }
        public string Carrier { get; set; } = string.Empty;
        public string? PrintData { get; set; }
        public bool IsPaymentDoor { get; set; }
        public bool PaymentType { get; set; }
        public string PackageNo { get; set; }
        public string? TrackingNumber { get; set; }
        public string? TrackingUrl { get; set; }
        public decimal? TotalWeight { get; set; }
        public DateTime? ShippedDateUtc { get; set; }
        public DateTime? DeliveryDateUtc { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public virtual ICollection<ShipmentItem> ShipmentItems { get; set; } = new HashSet<ShipmentItem>();
    }
}
