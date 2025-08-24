using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Product;

namespace Entegro.Application.DTOs.ProductCategory
{
    public class ProductCategoryDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public int DisplayOrder { get; set; }

        public CategoryDto Category { get; set; }
        public ProductDto Product { get; set; }
    }
}
