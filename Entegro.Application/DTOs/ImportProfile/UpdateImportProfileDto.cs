namespace Entegro.Application.DTOs.ImportProfile
{
    public class UpdateImportProfileDto
    {
        public int Id { get; set; }
        public string ProfileName { get; set; } = null!;
        public string ColumnMapping { get; set; } = null!;
        public string? MediaFileType { get; set; }
        public int MediaFileId { get; set; }
        public bool Enable { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}
