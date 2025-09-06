namespace Entegro.Web.Models.Catalog.Attributes
{
    public class ProductAttributeValueViewModel
    {
        public int Id { get; set; }
        public int ProductAttributeId { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
