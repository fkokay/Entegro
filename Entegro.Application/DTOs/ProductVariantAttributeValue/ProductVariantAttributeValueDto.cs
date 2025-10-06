namespace Entegro.Application.DTOs.ProductVariantAttributeValue
{
    public class ProductVariantAttributeValueDto
    {
        public int Id { get; set; }
        public int EntityId { get; set; }
        public int ProductVariantAttributeId { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
