namespace Entegro.Web.Models.Import
{
    public class ColumnMappingResult
    {
        public string ExcelHeader { get; set; }
        public string MappedName { get; set; }
        public string DefaultValue { get; set; } = "";
    }
}
