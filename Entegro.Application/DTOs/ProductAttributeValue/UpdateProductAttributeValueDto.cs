namespace Entegro.Application.DTOs.ProductAttributeValue
{
    public class UpdateProductAttributeValueDto
    {
        public int Id { get; set; }
        public int ProductAttributeId { get; set; }
        public string Name { get; set; } = null!;
        public int DisplayOrder { get; set; }
    }
}
