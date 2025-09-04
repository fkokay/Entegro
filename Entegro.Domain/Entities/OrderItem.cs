using Entegro.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entegro.Domain.Entities
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
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Price { get; set; }
        public decimal TaxRate { get; set; }
        public decimal DiscountAmount { get; set; }
        private Order? _order;
        public Order? Order
        {
            get => _order ?? LazyLoader?.Load(this, ref _order);
            set => _order = value;
        }
        private Product? _product;
        public Product? Product
        {
            get => _product ?? LazyLoader?.Load(this, ref _product);
            set => _product = value;
        }

    }
}
