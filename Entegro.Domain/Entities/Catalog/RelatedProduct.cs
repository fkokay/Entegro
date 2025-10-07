using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Domain.Entities.Catalog
{

    public class RelatedProductMap : IEntityTypeConfiguration<RelatedProduct>
    {
        public void Configure(EntityTypeBuilder<RelatedProduct> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Product1)
                .WithMany(p => p.RelatedProducts)
                .HasForeignKey(x => x.ProductId1)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Product2)
                .WithMany(p => p.RelatedAsSuggestedProduct)
                .HasForeignKey(x => x.ProductId2)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(x => new { x.ProductId1, x.ProductId2 }).IsUnique();
        }
    }

    [Table("RelatedProduct")]
    public class RelatedProduct : BaseEntity, IDisplayOrder
    {
        public int ProductId1 { get; set; }
        public int ProductId2 { get; set; }
        public int DisplayOrder { get; set; }

        public virtual Product Product1 { get; set; }
        public virtual Product Product2 { get; set; }

    }
}
