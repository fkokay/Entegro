namespace Entegro.Application.DTOs.ProductCategory
{
    public class CreateProductCategoryDto
    {
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public int DisplayOrder { get; set; }
    }
}
