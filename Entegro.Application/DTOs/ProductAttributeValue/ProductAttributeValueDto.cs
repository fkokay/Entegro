using Entegro.Application.DTOs.ProductAttribute;

namespace Entegro.Application.DTOs.ProductAttributeValue
{
    public class ProductAttributeValueDto
    {
        public int Id { get; set; }
        public int ProductAttributeId { get; set; }
        public string Name { get; set; } = null!;
        public int DisplayOrder { get; set; }

        public virtual ProductAttributeDto ProductAttribute { get; set; }
    }
}
