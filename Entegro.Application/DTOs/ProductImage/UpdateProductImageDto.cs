namespace Entegro.Application.DTOs.ProductImage
{
    public class UpdateProductImageDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int MediaFileId { get; set; }
        public int DisplayOrder { get; set; }

    }
}
