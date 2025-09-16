using Entegro.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities.Checkout
{
    public class ShipmentItemMap : IEntityTypeConfiguration<ShipmentItem>
    {
        public void Configure(EntityTypeBuilder<ShipmentItem> builder)
        {
            builder
              .HasOne(o => o.OrderItem)
              .WithMany()
              .HasForeignKey(o => o.OrderItemId)
              .OnDelete(DeleteBehavior.NoAction);

            builder
               .HasOne(x => x.Shipment)
               .WithMany(x => x.ShipmentItems)
               .HasForeignKey(x => x.ShipmentId)
               .OnDelete(DeleteBehavior.Cascade);

        }
    }
    [Table("ShipmentItem")]
    public class ShipmentItem : BaseEntity
    {
        public int ShipmentId { get; set; }
        public virtual Shipment Shipment { get; set; }
        public int OrderItemId { get; set; }
        public virtual OrderItem OrderItem { get; set; }
        public int Quantity { get; set; }
    }
}
