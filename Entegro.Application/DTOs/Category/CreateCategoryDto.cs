using Entegro.Application.DTOs.MediaFile;

namespace Entegro.Application.DTOs.Category
{
    public class CreateCategoryDto
    {
        public int? ParentId { get; set; }
        public CreateCategoryDto? Parent { get; set; }
        public string TreePath { get; set; } = string.Empty;
        public string Name { get; set; }
        public string? Description { get; set; }
        public int? MediaFileId { get; set; }
        public MediaFileDto? MediaFile { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }
        public int DisplayOrder { get; set; }
        public bool Published { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        public List<CreateCategoryDto> SubCategories { get; set; } = new List<CreateCategoryDto>();
    }
}
