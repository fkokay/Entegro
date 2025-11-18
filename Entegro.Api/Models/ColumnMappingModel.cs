namespace Entegro.Api.Models
{
    public class ColumnMappingModel
    {
        public string XmlUrl { get; set; }
        public List<MappingItem> Mappings { get; set; }
        public List<string> SelectedProducts { get; set; }
        public bool IncludeVariants { get; set; }
    }
    public class MappingItem
    {
        public string ColumnName { get; set; }
        public List<string> XmlTags { get; set; }
        public bool IsImage { get; set; }
    }


    public class VariantModel
    {
        public string ProductCode { get; set; }
        public string Barcode { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Dictionary<string, string> Specs { get; set; }
    }
}
