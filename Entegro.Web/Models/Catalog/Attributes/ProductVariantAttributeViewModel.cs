using Entegro.Web.Models.Catalog.Products;

namespace Entegro.Web.Models.Catalog.Attributes
{
    public class ProductVariantAttributeViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ProductAttributeId { get; set; }
        public string ProductAttribute { get; set; }
        public bool IsRequried { get; set; }
        public int AttributeControlTypeId { get; set; }
        public int DisplayOrder { get; set; }


        public ProductViewModel Product { get; set; }
        public ProductAttributeViewModel Attribute { get; set; }
        public List<ProductVariantAttributeValueViewModel> ProductVariantAttributeValues { get; set; } = new List<ProductVariantAttributeValueViewModel>();
    }
}
