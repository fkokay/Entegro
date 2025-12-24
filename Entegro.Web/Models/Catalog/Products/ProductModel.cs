using Entegro.Web.Models.Catalog.Attributes;
using Entegro.Web.Models.Catalog.Brands;
using Entegro.Web.Models.Content;

namespace Entegro.Web.Models.Catalog.Products
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }
        public decimal CostPrice { get; set; }
        public string? Currency { get; set; }
        public string? Unit { get; set; }
        public decimal VatRate { get; set; }
        public bool VatInc { get; set; }
        public int? BrandId { get; set; }
        public int StockQuantity { get; set; }
        public int MinStockQuantity { get; set; }
        public decimal Weight { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public string? MetaKeywords { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaTitle { get; set; }
        public string? Barcode { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string? ManufacturerPartNumber { get; set; }
        public string? Gtin { get; set; }
        public int? MainPictureId { get; set; }
        public bool Published { get; set; } = true;
        public BrandModel? Brand { get; set; }
        public string? PictureUrl { get; set; }
        public MediaFileModel? MainPicture { get; set; }
        public List<ProductMediaFileModel> ProductMediaFiles { get; set; } = new List<ProductMediaFileModel>();
        public List<ProductVariantAttributeModel> ProductVariantAttributes { get; set; } = new List<ProductVariantAttributeModel>();
        public List<ProductVariantAttributeCombinationModel> ProductVariantAttributeCombinations { get; set; } = new List<ProductVariantAttributeCombinationModel>();
    }
}
