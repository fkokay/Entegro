namespace Entegro.Application.DTOs.ProductSpecificationAttribute
{
    public class UpdateProductSpecificationAttributeDto
    {
        public int Id { get; set; }
        public int SpecificationAttributeOptionId { get; set; }
        public int ProductId { get; set; }
        public int DisplayOrder { get; set; }
    }
}
