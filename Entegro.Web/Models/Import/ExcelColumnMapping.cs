namespace Entegro.Web.Models.Import
{
    public class ExcelColumnMapping
    {
        public string ExcelHeader { get; set; }
        public bool IsImport { get; set; }
        public string DbColumn { get; set; }
        public string? DefaultValue { get; set; }
        public List<string> Values { get; set; } = new List<string>();
    }
}
