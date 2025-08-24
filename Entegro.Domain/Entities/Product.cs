using Entegro.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

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

        private ICollection<ProductVariantAttribute> _productVariantAttribute;
        public ICollection<ProductVariantAttribute> ProductVariantAttribute
        {
            get => LazyLoader?.Load(this, ref _productVariantAttribute) ?? (_productVariantAttribute ??= new HashSet<ProductVariantAttribute>());
            set => _productVariantAttribute = value;
        }

        private ICollection<ProductVariantAttributeCombination> _productVariantAttributeCombinations;
        public ICollection<ProductVariantAttributeCombination> ProductVariantAttributeCombination
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
