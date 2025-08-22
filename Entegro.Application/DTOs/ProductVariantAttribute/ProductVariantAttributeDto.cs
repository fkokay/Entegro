using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductAttribute;

namespace Entegro.Application.DTOs.ProductVariantAttribute
{
    public class ProductVariantAttributeDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ProductAttributeId { get; set; }
        public bool IsRequried { get; set; }
        public int AttributeControlTypeId { get; set; }
        public int DisplayOrder { get; set; }

        public ProductDto Product { get; set; }
        public ProductAttributeDto ProductAttribute { get; set; }
    }
}
