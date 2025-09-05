
using Entegro.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Domain.Entities
{
    public class OrderMap : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(p => p.TotalAmount).HasPrecision(18, 4);
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
        public string OrderNo { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public bool Deleted { get; set; }
        public bool IsTransient { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
