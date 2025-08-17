namespace Entegro.Application.DTOs.ProductAttribute
{
    public class CreateProductAttributeDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
    }
}
