using Entegro.Application.DTOs.ProductMediaFile;
using Entegro.Application.DTOs.ProductVariantAttributeCombination;

namespace Entegro.Application.DTOs.Product
{
    public class UpdateProductDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Currency { get; set; }
        public string? Unit { get; set; }
        public decimal VatRate { get; set; }
        public bool VatInc { get; set; }
        public int? BrandId { get; set; }
        public int StockQuantity { get; set; }
        public decimal Weight { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public string? MetaKeywords { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaTitle { get; set; }
        public string? Barcode { get; set; }
        public int? MainPictureId { get; set; }
        public string? ManufacturerPartNumber { get; set; }
        public string? Gtin { get; set; }
        public bool Published { get; set; } = true;
        public List<ProductMediaFileDto> ProductMediaFiles { get; set; } = new List<ProductMediaFileDto>();
        public List<ProductVariantAttributeCombinationDto> ProductVariantAttributeCombinations { get; set; } = new List<ProductVariantAttributeCombinationDto>();
    }
}
