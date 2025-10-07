using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.SpecificationAttributeOption;

namespace Entegro.Application.DTOs.ProductSpecificationAttribute
{
    public class ProductSpecificationAttributeDto
    {
        public int Id { get; set; }
        public int SpecificationAttributeOptionId { get; set; }
        public SpecificationAttributeOptionDto SpecificationAttributeOption { get; set; }
        public int ProductId { get; set; }
        public ProductDto Product { get; set; }
        public int DisplayOrder { get; set; }
    }
}
