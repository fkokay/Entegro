namespace Entegro.Application.DTOs.ProductVariantAttribute
{
    public class UpdateProductVariantAttributeDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ProductAttributeId { get; set; }
        public bool IsRequried { get; set; }
        public int AttributeControlTypeId { get; set; }
        public int DisplayOrder { get; set; }

    }
}
