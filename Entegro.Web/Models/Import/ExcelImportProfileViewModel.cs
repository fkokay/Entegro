namespace Entegro.Web.Models.Import
{
    public class ExcelImportProfileViewModel
    {
        public List<ColumnMapping> ColumnMappings { get; set; } = new List<ColumnMapping>();
        public int MediaFileId { get; set; }
        public string ProfileName { get; set; }
        public string? MediaFileType { get; set; }
        public bool Enable { get; set; } = true;
    }
}
