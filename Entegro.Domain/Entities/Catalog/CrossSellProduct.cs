using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Domain.Entities.Catalog
{

    public class CrossSellProductMap : IEntityTypeConfiguration<CrossSellProduct>
    {
        public void Configure(EntityTypeBuilder<CrossSellProduct> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Product1)
                .WithMany(p => p.CrossSellProducts)
                .HasForeignKey(x => x.ProductId1)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Product2)
                .WithMany(p => p.CrossSellAsSuggestedProduct)
                .HasForeignKey(x => x.ProductId2)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(x => new { x.ProductId1, x.ProductId2 }).IsUnique();
        }
    }

    [Table("CrossSellProduct")]
    public class CrossSellProduct : BaseEntity
    {
        public int ProductId1 { get; set; }
        public int ProductId2 { get; set; }


        public virtual Product Product1 { get; set; }
        public virtual Product Product2 { get; set; }
    }
}
