namespace Entegro.Web.Models.Catalog.ProductSpecificationAttribute
{
    public class UpdateProductSpecificationAttributeModel
    {
        public int Id { get; set; }
        public int SpecificationAttributeOptionId { get; set; }
        public int ProductId { get; set; }
        public int DisplayOrder { get; set; }
    }
}
