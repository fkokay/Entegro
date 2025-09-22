using Entegro.Web.Models.Catalog.SpecificationAttributeOptions;

namespace Entegro.Web.Models.Catalog.SpecificationAttributes
{
    public class SpecificationAttributeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<SpecificationAttributeOptionModel>? SpecificationAttributeOptions { get; set; }
    }
}
