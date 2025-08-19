namespace Entegro.Web.Models
{
    public class UpdateProductAttributeValueViewModel
    {
        public int Id { get; set; }
        public int ProductAttributeId { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
