using Entegro.Application.DTOs.ProductVariantAttribute;

namespace Entegro.Application.DTOs.ProductVariantAttributeValue
{
    public class CreateProductVariantAttributeValueDto
    {
        public int ProductVariantAttributeId { get; set; }
        public string Name { get; set; }
        public ProductVariantAttributeDto? ProductVariantAttribute { get; set; }
        public int DisplayOrder { get; set; }
    }
}
