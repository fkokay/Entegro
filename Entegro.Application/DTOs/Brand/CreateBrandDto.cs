using Entegro.Application.DTOs.MediaFile;

namespace Entegro.Application.DTOs.Brand
{
    public class CreateBrandDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int? MediaFileId { get; set; }
        public MediaFileDto? MediaFile { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }
        public int DisplayOrder { get; set; }
        public bool Published { get; set; }
    }
}
