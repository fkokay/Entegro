namespace Entegro.Application.DTOs.ProductImage
{
    public class ProductImageDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int MediaFileId { get; set; }
        public int DisplayOrder { get; set; }
        public string? Url { get; set; }

    }
}
