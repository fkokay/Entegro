using Entegro.Application.DTOs.MediaFile;
using Entegro.Application.DTOs.Product;

namespace Entegro.Application.DTOs.ProductMediaFile
{
    public class ProductMediaFileDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int MediaFileId { get; set; }
        public int DisplayOrder { get; set; }

        public ProductDto Product { get; set; }
        public MediaFileDto MediaFile { get; set; }

    }
}
