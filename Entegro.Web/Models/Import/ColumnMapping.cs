namespace Entegro.Web.Models.Import
{
    public class ColumnMapping
    {

        public string ExcelHeader { get; set; }
        public bool IsImport { get; set; }
        public string DbColumn { get; set; }

        public List<string> Values { get; set; } = new List<string>();
    }
}
