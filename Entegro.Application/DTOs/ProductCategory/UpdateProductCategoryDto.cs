namespace Entegro.Application.DTOs.ProductCategory
{
    public class UpdateProductCategoryDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public int DisplayOrder { get; set; }
    }
}
