namespace Entegro.Application.DTOs.ProductAttribute
{
    public class UpdateProductAttributeDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
    }
}
