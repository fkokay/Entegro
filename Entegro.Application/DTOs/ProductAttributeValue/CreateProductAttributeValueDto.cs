namespace Entegro.Application.DTOs.ProductAttributeValue
{
    public class CreateProductAttributeValueDto
    {
        public int ProductAttributeId { get; set; }
        public string Name { get; set; } = null!;
        public int DisplayOrder { get; set; }
    }
}
