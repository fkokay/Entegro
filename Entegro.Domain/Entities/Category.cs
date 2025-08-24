using Entegro.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entegro.Domain.Entities
{
    public class CategoryMap : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasOne(c => c.ParentCategory)
                   .WithMany()
                   .HasForeignKey(c => c.ParentCategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.ProductCategories)
                   .WithOne(pc => pc.Category)
                   .HasForeignKey(pc => pc.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

    [Table("Category")]
    public class Category : BaseEntity, IDisplayOrder
    {
        public int? ParentCategoryId { get; set; }
        private Category? _parentCategory;
        public Category? ParentCategory
        {
            get => _parentCategory ?? LazyLoader?.Load(this, ref _parentCategory);
            set => _parentCategory = value;
        }
        public string TreePath { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int? MediaFileId { get; set; }

        private MediaFile? _mediaFile;
        public MediaFile? MediaFile
        {
            get => _mediaFile ?? LazyLoader?.Load(this, ref _mediaFile);
            set => _mediaFile = value;
        }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        private ICollection<ProductCategory> _productCategories;
        public ICollection<ProductCategory> ProductCategories
        {
            get => LazyLoader?.Load(this, ref _productCategories) ?? (_productCategories ??= new HashSet<ProductCategory>());
            set => _productCategories = value;
        }
    }

    public sealed class CategorySlim
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? TreePath { get; set; }
    }

    public sealed class PagedResult2<T>
    {
        public List<T> Items { get; init; } = new();
        public bool HasMore { get; init; }
    }

}
