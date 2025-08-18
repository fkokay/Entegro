namespace Entegro.Web.Models
{
    public class ProductAttributeValueViewModel
    {
        public int Id { get; set; }
        public int ProductAttributeId { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
    public class CreateProductAttributeValueViewModel
    {
        public int ProductAttributeId { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
    public class UpdateProductAttributeValueViewModel
    {
        public int Id { get; set; }
        public int ProductAttributeId { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
