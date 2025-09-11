using Entegro.Application.DTOs.Product;
using Entegro.Web.Models.Catalog.SpecificationAttributeOptions;

namespace Entegro.Web.Models.Catalog.ProductSpecificationAttribute
{
    public class ProductSpecificationAttributeViewModel
    {
        public int Id { get; set; }
        public int SpecificationAttributeOptionId { get; set; }
        public virtual SpecificationAttributeOptionViewModel SpecificationAttributeOption { get; set; }
        public int ProductId { get; set; }
        public virtual ProductDto Product { get; set; }
        public int DisplayOrder { get; set; }
    }
}
