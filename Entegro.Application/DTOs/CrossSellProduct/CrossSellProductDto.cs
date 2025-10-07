using Entegro.Application.DTOs.Product;

namespace Entegro.Application.DTOs.CrossSellProduct
{
    public class CrossSellProductDto
    {
        public int Id { get; set; }
        public int ProductId1 { get; set; }
        public int ProductId2 { get; set; }

        public ProductDto Product1 { get; set; }
        public ProductDto Product2 { get; set; }
    }
}
