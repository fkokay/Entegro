using Entegro.Domain.Entities.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Emit;

namespace Entegro.Domain.Entities.Catalog
{
    public class ProductMap : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasMany(p => p.ProductCategories)
                   .WithOne(pc => pc.Product)
                   .HasForeignKey(pc => pc.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(p => p.Price).HasPrecision(18, 4);
            builder.Property(p => p.OldPrice).HasPrecision(18, 4);
            builder.Property(p => p.SpecialPrice).HasPrecision(18, 4);
            builder.Property(p => p.VatRate).HasPrecision(18, 4);
            builder.Property(p => p.Weight).HasPrecision(18, 4);
            builder.Property(p => p.Length).HasPrecision(18, 4);
            builder.Property(p => p.Width).HasPrecision(18, 4);
            builder.Property(p => p.Height).HasPrecision(18, 4);
        }
    }

    [Table("Product")]
    public class Product : BaseEntity, ISoftDeletable, IAuditable
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ManufacturerPartNumber { get; set; }
        public string? Gtin { get; set; }
        public decimal Price { get; set; }
        public decimal OldPrice { get; set; }
        public decimal SpecialPrice { get; set; }
        public string? Currency { get; set; }
        public string? Unit { get; set; }
        public decimal VatRate { get; set; }
        public bool VatInc { get; set; }
        public int? BrandId { get; set; }
        public virtual Brand? Brand { get; set; }
        public int StockQuantity { get; set; }
        public decimal Weight { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public string? MetaKeywords { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaTitle { get; set; }
        public string? Barcode { get; set; }
        public int? MainPictureId { get; set; }
        public virtual MediaFile? MainPicture { get; set; }
        public bool Published { get; set; } = true;
        public bool Deleted { get; set; } = false;
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public virtual ICollection<ProductMediaFile> ProductMediaFiles { get; set; } = new HashSet<ProductMediaFile>();
        public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new HashSet<ProductCategory>();
        public virtual ICollection<ProductVariantAttribute> ProductVariantAttributes { get; set; } = new HashSet<ProductVariantAttribute>();
        public virtual ICollection<ProductVariantAttributeCombination> ProductVariantAttributeCombinations { get; set; } = new HashSet<ProductVariantAttributeCombination>();
        public virtual ICollection<ProductIntegration> ProductIntegrations { get; set; } = new HashSet<ProductIntegration>();
    }
}
