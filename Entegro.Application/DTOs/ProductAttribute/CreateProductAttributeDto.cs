using Entegro.Application.DTOs.ProductAttributeValue;

namespace Entegro.Application.DTOs.ProductAttribute
{
    public class CreateProductAttributeDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }

        public IList<ProductAttributeValueDto> ProductAttributeValues { get; set; } = new List<ProductAttributeValueDto>(); 
    }
}
