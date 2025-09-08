using Entegro.Web.Models.Catalog.Categories;

namespace Entegro.Web.Models.Catalog.Products
{
    public class ProductCategoryViewModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public int DisplayOrder { get; set; }
        public CategoryViewModel Category { get; set; }

    }
}
