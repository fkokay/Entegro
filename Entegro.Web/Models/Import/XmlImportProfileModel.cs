namespace Entegro.Web.Models.Import
{
    public class XmlImportProfileModel
    {
        public int Id { get; set; }
        public string? MediaFileUrl { get; set; }
        public string ProfileName { get; set; } = null!;
        public string? MediaFileType { get; set; }
        public List<string> Paths { get; set; } = new();
        public bool Enable { get; set; } = true;
        public string? Error { get; set; }
        public string? PreviewXml { get; set; }

        public string? PriceAdjustmentType { get; set; }
        public decimal? PriceAdjustmentAmount { get; set; }
        public decimal? OptionalExtraAmount { get; set; }
        public bool? ApplyPriceAdjustment { get; set; }

        public bool? IsVariantProduct { get; set; }
        public string? AttributeStockCode { get; set; }
        public string? AttributePrice { get; set; }
        public string? AttributeGtin { get; set; }
        public string? AttributeManufacturerPartNumber { get; set; }
        public string? AttributeStockQuantity { get; set; }
        public string? AttributeSpecifications { get; set; }
        public string? RowAttribute { get; set; }
        public string? AssignedMediaFileIds { get; set; } = "string";


        public CreateXmlProductImportProfileModel ProductImport { get; set; } = new();
    }
}
