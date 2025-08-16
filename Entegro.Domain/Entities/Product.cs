using Entegro.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities
{
    [Table("Product")]
    public class Product : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Currency { get; set; }
        public string? Unit { get; set; }
        public decimal VatRate { get; set; }
        public bool VatInc { get; set; }
        public int? BrandId { get; set; }
        public int StockQuantity { get; set; }
        public decimal Weight { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public string? MetaKeywords { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaTitle { get; set; }
        public string? Barcode { get; set; }

        public DateTime CreatedOn { get; set; } 
        public DateTime UpdatedOn { get; set; }

        public virtual Brand? Brand { get; set; }
        public virtual ICollection<ProductImageMapping> ProductImages { get; set; } = new List<ProductImageMapping>();
    }
}
