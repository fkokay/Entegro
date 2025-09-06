using Entegro.Application.DTOs.MediaFile;
using Entegro.Application.DTOs.Product;

namespace Entegro.Application.DTOs.ProductMediaFile
{
    public class CreateProductMediaFileDto
    {
        public int ProductId { get; set; }
        public ProductDto Product {get;set;}
        public int MediaFileId { get; set; }
        public MediaFileDto MediaFile { get; set; }
        public int DisplayOrder { get; set; }

    }
}
