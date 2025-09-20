namespace Entegro.Web.Models.Catalog.SpecificationAttributeOptions
{
    public class UpdateSpecificationAttributeOptionModel
    {
        public int Id { get; set; }
        public int SpecificationAttributeId { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
