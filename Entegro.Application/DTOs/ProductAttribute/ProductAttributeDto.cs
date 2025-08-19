using Entegro.Application.DTOs.ProductAttributeValue;

namespace Entegro.Application.DTOs.ProductAttribute
{
    public class ProductAttributeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }

        public IList<ProductAttributeValueDto> Values { get; set; }
    }
}
