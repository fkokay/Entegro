using Entegro.Domain.Entities.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
namespace Entegro.Domain.Entities.Catalog
{
    public class CategoryMap : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasOne(c => c.Parent)
                   .WithMany(c => c.Children)
                   .HasForeignKey(c => c.ParentId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(c => c.MediaFile)
                    .WithMany()
                    .HasForeignKey(c => c.MediaFileId)
                    .OnDelete(DeleteBehavior.SetNull);

        }
    }

    [Table("Category")]
    public class  Category : BaseEntity, ITreeNode, ICategoryNode, ISoftDeletable,IDisplayOrder, IAuditable
    {
        [Column("ParentCategoryId")]
        public int? ParentId { get; set; }

        public IEnumerable<ITreeNode> GetChildNodes() => Children;
        public ITreeNode? GetParentNode() => Parent;

        [IgnoreDataMember]
        public virtual Category? Parent { get; set; }
        public string TreePath { get; set; } = string.Empty;
        public string Name { get; set; }
        public string? Description { get; set; }

        public int? MediaFileId { get; set; }
        public virtual MediaFile? MediaFile { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }
        public int DisplayOrder { get; set; }
        public bool Deleted { get; set; }
        public bool Published { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }

        [IgnoreDataMember]
        public virtual ICollection<Category> Children { get; set; } = new HashSet<Category>();

    }
}
