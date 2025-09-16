namespace Entegro.Application.DTOs.ImportProfile
{
    public class CreateImportProfileDto
    {
        public string ProfileName { get; set; } = null!;
        public string ColumnMapping { get; set; } = null!;
        public string? MediaFileType { get; set; }
        public string? MediaFileUrl { get; set; }
        public int MediaFileId { get; set; }
        public bool Enable { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}
