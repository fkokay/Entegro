namespace Entegro.Web.Models.Import
{
    public class ExcelImportProfileViewModel
    {
        public List<ExcelColumnMapping> ColumnMappings { get; set; } = new List<ExcelColumnMapping>();
        public int MediaFileId { get; set; }
        public string ProfileName { get; set; }
        public string? MediaFileType { get; set; }
        public bool Enable { get; set; } = true;
    }
}
