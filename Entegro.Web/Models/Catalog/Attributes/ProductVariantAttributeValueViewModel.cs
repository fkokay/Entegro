using Entegro.Domain.Entities.Catalog;

namespace Entegro.Web.Models.Catalog.Attributes
{
    public class ProductVariantAttributeValueViewModel
    {
        public int Id { get; set; }
        public int ProductVariantAttributeId { get; set; }
        public string Name { get; set; }
    }
}
