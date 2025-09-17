namespace Entegro.Web.Models.Import
{
    public class XmlImportProfileViewModel
    {
        public string? MediaFileUrl { get; set; }
        public string ProfileName { get; set; } = null!;
        public string? MediaFileType { get; set; }
        public List<string> Paths { get; set; } = new();
        public bool Enable { get; set; } = true;
        public string? Error { get; set; }
        public string? PreviewXml { get; set; }

        //field

        public string? Name { get; set; }
        public string? Brand { get; set; }
        public string? Barcode { get; set; }
        public string? Code { get; set; }
        public string? ManufacturerPartNumber { get; set; }
        public string? Gtin { get; set; }
        public string? Price { get; set; }
        public string? Currency { get; set; }
        public string? Unit { get; set; }
        public string? VatRate { get; set; }
        public string? VatInc { get; set; }
        public string? StockQuantity { get; set; }
        public string? Weight { get; set; }
        public string? Length { get; set; }
        public string? Width { get; set; }
        public string? Height { get; set; }
        public string? MetaKeywords { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaTitle { get; set; }
        public string? Description { get; set; }
        public string? Images { get; set; }
        public string? Categories { get; set; }
        public bool? PriceAdjustmentType { get; set; }
        public decimal? PriceAdjustmentAmount { get; set; }
        public decimal? OptionalExtraAmount { get; set; }
        public bool? ApplyPriceAdjustment { get; set; }

    }
    public class HeaderMap
    {
        public string XmlHeader { get; set; }
        public string MappedName { get; set; }
    }
}
