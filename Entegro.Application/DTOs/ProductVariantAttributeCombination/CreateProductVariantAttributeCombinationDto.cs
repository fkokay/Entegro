namespace Entegro.Application.DTOs.ProductVariantAttributeCombination
{
    internal class CreateProductVariantAttributeCombinationDto
    {

        public int ProductId { get; set; }
        public string StokCode { get; set; }
        public string Gtin { get; set; }
        public string ManufacturerPartNumber { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string AttributeXml { get; set; }

    }
}
