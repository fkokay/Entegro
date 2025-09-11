namespace Entegro.Application.DTOs.ProductSpecificationAttribute
{
    public class CreateProductSpecificationAttributeDto
    {
        public int SpecificationAttributeOptionId { get; set; }
        public int ProductId { get; set; }
        public int DisplayOrder { get; set; }
    }
}
