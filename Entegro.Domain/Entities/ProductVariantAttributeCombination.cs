using Entegro.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
