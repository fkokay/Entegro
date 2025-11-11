namespace Entegro.Web.Models.Import
{
    public class DbColumn
    {
        public string ColumnName { get; set; }
        public List<string> XmlTags { get; set; } = new();
        public bool IsImage { get; set; }
    }


    public class XmlMapping
    {
        public string ColumnName { get; set; }
        public List<string> XmlTags { get; set; } = new();
        public bool IsImage { get; set; }
    }



    public class XmlStructure
    {
        public string RootName { get; set; }
        public List<string> Tags { get; set; } = new();
    }


    public class ProductRow
    {
        public string ProductCode { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Price { get; set; }
        public string Currency { get; set; }
        public string Brand { get; set; }
        public string Stock { get; set; }
        public string VariantInfo { get; set; }
    }


    public class MappingSaveRequest
    {
        public int ProfileId { get; set; }
        public string XmlUrl { get; set; }
        public List<XmlMapping> Mappings { get; set; }
        public List<string> SelectedProducts { get; set; }
        public bool IncludeVariants { get; set; }
    }
}

