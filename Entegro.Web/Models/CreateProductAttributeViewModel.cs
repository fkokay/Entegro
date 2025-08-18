namespace Entegro.Web.Models
{
    public class CreateProductAttributeViewModel
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
    }
}
