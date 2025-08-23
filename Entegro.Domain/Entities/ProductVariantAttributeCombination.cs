using Entegro.Domain.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entegro.Domain.Entities
{
    [Table("ProductVariantAttributeCombination")]
    public class ProductVariantAttributeCombination : BaseEntity
    {
        public int ProductId { get; set; }
        public string StokCode { get; set; }
        public string Gtin { get; set; }
        public string ManufacturerPartNumber { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string AttributeXml { get; set; }


        private Product _product;

        public Product Product
        {
            get => _product ?? LazyLoader.Load(this, ref _product);
            set => _product = value;
        }

    }
}
