namespace Entegro.Application.DTOs.ProductMediaFile
{
    public class UpdateProductMediaFileDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int MediaFileId { get; set; }
        public int DisplayOrder { get; set; }

    }
}
