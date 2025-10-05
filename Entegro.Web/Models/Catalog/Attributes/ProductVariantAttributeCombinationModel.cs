using Entegro.Application.DTOs.Common;

namespace Entegro.Web.Models.Catalog.Attributes
{
    public class ProductVariantAttributeCombinationModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? StokCode { get; set; }
        public string? Gtin { get; set; }
        public string? ManufacturerPartNumber { get; set; }
        public decimal? Price { get; set; }
        public int StockQuantity { get; set; }
        public int[] AssignedPictureIds { get; set; } = [];

        public List<ProductVariantAttributeSelection> ProductVariantAttributeSelections { get; set; } = new List<ProductVariantAttributeSelection>();
    }
}
