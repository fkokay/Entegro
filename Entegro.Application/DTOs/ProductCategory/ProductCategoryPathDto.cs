namespace Entegro.Application.DTOs.ProductCategory
{
    public class ProductCategoryPathDto
    {
        public int Id { get; set; }
        public int ProductId { get; init; }
        public int CategoryId { get; init; }
        public int DisplayOrder { get; init; }
        public string CategoryPath { get; init; } = string.Empty;
    }
}
