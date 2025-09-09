using Entegro.Application.DTOs.Common;

namespace Entegro.Web.Models.Catalog.Attributes
{
    public class ProductVariantAttributeCombinationViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string? StokCode { get; set; }
        public string? Gtin { get; set; }
        public string? ManufacturerPartNumber { get; set; }
        public decimal Price { get; set; } = 0;
        public int StockQuantity { get; set; } = 0;

        public List<ProductVariantAttributeSelection> Attributes { get; set; } = new List<ProductVariantAttributeSelection>();
    }
}
