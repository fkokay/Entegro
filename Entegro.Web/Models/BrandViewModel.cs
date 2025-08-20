using Entegro.Application.DTOs.MediaFile;

namespace Entegro.Web.Models
{
    public class BrandViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int? MediaFileId { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        public MediaFileViewModel? MediaFile { get; set; }
    }
}
