using Entegro.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Emit;

namespace Entegro.Domain.Entities
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

        private Brand? _brand;
        public Brand? Brand
        {
            get => _brand ?? LazyLoader?.Load(this, ref _brand);
            set => _brand = value;
        }


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
        private MediaFile? _mainPicture;
        public MediaFile? MainPicture
        {
            get => _mainPicture ?? LazyLoader?.Load(this, ref _mainPicture);
            set => _mainPicture = value;
        }
        public bool Published { get; set; } = true;
        public bool Deleted { get; set; } = false;
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        private ICollection<ProductMediaFile> _productMediaFiles;
        public ICollection<ProductMediaFile> ProductMediaFiles
        {
            get => LazyLoader?.Load(this, ref _productMediaFiles) ?? (_productMediaFiles ??= new HashSet<ProductMediaFile>());
            set => _productMediaFiles = value;
        }

        private ICollection<ProductCategory> _productCategories;
        public ICollection<ProductCategory> ProductCategories
        {
            get => LazyLoader?.Load(this, ref _productCategories) ?? (_productCategories ??= new HashSet<ProductCategory>());
            set => _productCategories = value;
        }

        private ICollection<ProductVariantAttribute> _productVariantAttributes;
        public ICollection<ProductVariantAttribute> ProductVariantAttributes
        {
            get => LazyLoader?.Load(this, ref _productVariantAttributes) ?? (_productVariantAttributes ??= new HashSet<ProductVariantAttribute>());
            set => _productVariantAttributes = value;
        }

        private ICollection<ProductVariantAttributeCombination> _productVariantAttributeCombinations;
        public ICollection<ProductVariantAttributeCombination> ProductVariantAttributeCombinations
        {
            get => LazyLoader?.Load(this, ref _productVariantAttributeCombinations) ?? (_productVariantAttributeCombinations ??= new HashSet<ProductVariantAttributeCombination>());
            set => _productVariantAttributeCombinations = value;
        }

        private ICollection<ProductIntegration> _productIntegrations;
        public ICollection<ProductIntegration> ProductIntegrations
        {
            get => LazyLoader?.Load(this, ref _productIntegrations) ?? (_productIntegrations ??= new HashSet<ProductIntegration>());
            set => _productIntegrations = value;
        }

    }
}
