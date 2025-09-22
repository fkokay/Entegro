namespace Entegro.Web.Models.Import
{
    public class CreateXmlProductImportProfileModel
    {
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
    }
}
