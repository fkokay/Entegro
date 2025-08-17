namespace Entegro.Application.DTOs.ProductAttributeMapping
{
    public class UpdateProductAttributeMappingDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ProductAttributeId { get; set; }
        public bool IsRequried { get; set; }
        public int AttributeControlTypeId { get; set; }
        public int DisplayOrder { get; set; }
    }
}
