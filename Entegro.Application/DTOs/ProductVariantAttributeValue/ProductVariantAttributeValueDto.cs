using Entegro.Application.DTOs.ProductVariantAttribute;

namespace Entegro.Application.DTOs.ProductVariantAttributeValue
{
    public class ProductVariantAttributeValueDto
    {
        public int ProductVariantAttributeId { get; set; }
        public ProductVariantAttributeDto ProductVariantAttribute { get; set; }
        public string Name { get; set; }
    }
}
