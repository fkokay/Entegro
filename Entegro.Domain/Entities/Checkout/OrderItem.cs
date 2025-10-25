using Entegro.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entegro.Domain.Entities.Checkout
{
    public class OrderItemMap : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.Property(p => p.UnitPrice).HasPrecision(18, 4);
            builder.Property(p => p.Price).HasPrecision(18, 4);
            builder.Property(p => p.TaxRate).HasPrecision(18, 4);
            builder.Property(p => p.DiscountAmount).HasPrecision(18, 4);
        }
    }

    [Table("OrderItem")]
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public int? ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public string Sku { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Price { get; set; }
        public decimal TaxRate { get; set; }
        public decimal DiscountAmount { get; set; }
        public string? AttributesDescription { get; set; }
        public string? AttributesXml { get; set; }
        public decimal ItemWeight { get; set; }
        public decimal ProductCost { get; set; }
        public string? IntegrationSku { get; set; }
        public string? IntegrationProductName { get; set; }
        public string? IntegrationProductImageUrl { get; set; }//pazaryerinden gelen ve eşleştirilmeyen ürün resmi

        public virtual ICollection<ShipmentItem> ShipmentItems { get; set; } = new List<ShipmentItem>();

    }
}
