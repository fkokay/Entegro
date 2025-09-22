namespace Entegro.Web.Models.Catalog.Products
{
    public class ProductCategoryModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public int DisplayOrder { get; set; }
        public string CategoryBreadcrumb { get; set; } = string.Empty;

    }
}
