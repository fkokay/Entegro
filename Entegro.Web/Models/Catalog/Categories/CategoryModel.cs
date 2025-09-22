using Entegro.Application.DTOs.MediaFile;
using Entegro.Web.Models.Content;

namespace Entegro.Web.Models.Catalog.Categories
{
    public class CategoryModel
    {
        public int Id { get; set; }
        public int? ParentCategoryId { get; set; }
        public int? MediaFileId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }
        public int DisplayOrder { get; set; }
        public bool Published { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public CategoryModel? Parent { get; set; }
        public MediaFileModel? MediaFile { get; set; }

    }
}
