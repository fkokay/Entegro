using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.ProductBrand
{
    public class ProductBrandDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ManufacturerId { get; set; }
        public bool IsFeaturedProduct { get; set; }
        public int DisplayOrder { get; set; }

    }
}
