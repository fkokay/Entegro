using Entegro.Web.Models.Catalog.SpecificationAttributeOptions;

namespace Entegro.Web.Models.Catalog.SpecificationAttributes
{
    public class SpecificationAttributeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<SpecificationAttributeOptionViewModel>? SpecificationAttributeOptions { get; set; }
    }
}
