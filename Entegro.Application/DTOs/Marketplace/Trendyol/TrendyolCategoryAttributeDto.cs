namespace Entegro.Application.DTOs.Marketplace.Trendyol
{
    public class TrendyolCategoryAttributeDto
    {
        public int CategoryId { get; set; }
        public TrendyolAttributeDetailDto Attribute { get; set; } = new();
        public bool Required { get; set; }
        public bool AllowCustom { get; set; }
        public bool Varianter { get; set; }
        public bool Slicer { get; set; }
        public bool AllowMultipleAttributeValues { get; set; }
        public List<TrendyolCategoryAttributeValueDto> AttributeValues { get; set; } = new();
    }

    public class TrendyolVariantDto
    {
        public string Barcode { get; set; }
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public decimal ListPrice { get; set; }
        public List<TrendyolProductAttributeDto> VariantAttributes { get; set; } = new();
    }
}
