using Entegro.Application.DTOs.SpecificationAttributeOption;

namespace Entegro.Application.DTOs.SpecificationAttribute
{
    public class SpecificationAttributeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<SpecificationAttributeOptionDto> SpecificationAttributeOptions { get; set; } = new List<SpecificationAttributeOptionDto>();
    }
}
