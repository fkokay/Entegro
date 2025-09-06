namespace Entegro.Web.Models.Catalog.Attributes
{
    public class ProductAttributeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }

        public List<ProductAttributeValueViewModel> Values { get; set; }
    }
}
