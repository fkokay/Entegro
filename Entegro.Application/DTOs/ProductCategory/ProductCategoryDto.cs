using Entegro.Application.DTOs.Category;

namespace Entegro.Application.DTOs.ProductCategory
{
    public class ProductCategoryDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public int DisplayOrder { get; set; }
        public string CategoryBreadcrumb { get; set; }

        public CategoryDto Category { get; set; }
    }
}
