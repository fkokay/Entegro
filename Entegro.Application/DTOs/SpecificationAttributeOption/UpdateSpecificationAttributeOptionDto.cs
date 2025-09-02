namespace Entegro.Application.DTOs.SpecificationAttributeOption
{
    public class UpdateSpecificationAttributeOptionDto
    {
        public int Id { get; set; }
        public int SpecificationAttributeId { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
