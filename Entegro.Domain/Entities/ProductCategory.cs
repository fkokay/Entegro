using Entegro.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entegro.Domain.Entities
{
    public class ProductCategoryMap : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            builder.HasKey(pc => pc.Id);

            builder.HasOne(c => c.Category)
                .WithMany(c => c.ProductCategories)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Product)
                .WithMany(c => c.ProductCategories)
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    [Table("Product_Category_Mapping")]
    [Index(nameof(CategoryId), Name = "IX_CategoryId")]
    public class ProductCategory : BaseEntity, IDisplayOrder
    {
        public int CategoryId { get; set; }

        private Category _category;
        public Category Category
        {
            get => _category ?? LazyLoader.Load(this, ref _category);
            set => _category = value;
        }

        public int ProductId { get; set; }

        private Product _product;
        public Product Product
        {
            get => _product ?? LazyLoader.Load(this, ref _product);
            set => _product = value;
        }
        public int DisplayOrder { get; set; }

    }
}
