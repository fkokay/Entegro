namespace Entegro.Application.DTOs.ProductAttributeMapping
{
    public class CreateProductAttributeMappingDto
    {
        public int ProductId { get; set; }
        public int ProductAttributeId { get; set; }
        public bool IsRequried { get; set; }
        public int AttributeControlTypeId { get; set; }
        public int DisplayOrder { get; set; }
    }
}
