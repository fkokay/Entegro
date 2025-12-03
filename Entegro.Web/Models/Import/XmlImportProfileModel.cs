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
        public string? AssignedMediaFileIds { get; set; } = "";

        public string? SelectedImagePaths { get; set; }
        public string? SelectedAttributeSpecifications { get; set; }
        public int VariantCount { get; set; }

        public CreateXmlProductImportProfileModel ProductImport { get; set; } = new();


        public List<IntegrationProfileLine> ECommerces { get; set; } = new();
        public List<IntegrationProfileLine> Marketplaces { get; set; } = new();
    }

    public class IntegrationProfileLine
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public decimal ProfitRate { get; set; }
        public decimal Commission { get; set; }
        public decimal CargoPrice { get; set; }
        public decimal ExtraCost { get; set; }
        public bool ApplyAutoPrice { get; set; } = true;
    }

    public class PlatformStoreRoot
    {
        public List<PlatformStoreItem> PlatformStore { get; set; } = new();
    }

    public class PlatformStoreItem
    {
        public int id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string value { get; set; }
        public Pricing pricing { get; set; }
    }

    public class Pricing
    {
        public decimal profitRate { get; set; }
        public decimal commission { get; set; }
        public decimal cargoPrice { get; set; }
        public decimal extraCost { get; set; }
        public bool applyAutoPrice { get; set; }
    }

}
