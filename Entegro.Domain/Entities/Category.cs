using Entegro.Domain.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entegro.Domain.Entities
{
    [Table("Category")]
    public class Category : BaseEntity, IDisplayOrder
    {
        public int? ParentCategoryId { get; set; }
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
