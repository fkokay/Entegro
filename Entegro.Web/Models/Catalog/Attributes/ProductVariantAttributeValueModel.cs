namespace Entegro.Web.Models.Catalog.Attributes
{
    public class ProductVariantAttributeValueModel
    {
        public int Id { get; set; }
        public int ProductVariantAttributeId { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
