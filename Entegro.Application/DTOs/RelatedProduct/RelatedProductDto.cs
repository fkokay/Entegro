using Entegro.Application.DTOs.Product;

namespace Entegro.Application.DTOs.RelatedProduct
{
    public class RelatedProductDto
    {
        public int Id { get; set; }
        public int ProductId1 { get; set; }
        public int ProductId2 { get; set; }
        public int DisplayOrder { get; set; }

        public ProductDto Product1 { get; set; }
        public ProductDto Product2 { get; set; }
    }
}
