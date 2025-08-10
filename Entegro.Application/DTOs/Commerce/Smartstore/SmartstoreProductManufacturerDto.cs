using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Commerce.Smartstore
{
    public class SmartstoreProductManufacturerDto
    {
        public int ManufacturerId { get; set; } 
        public int ProductId { get; set; }
        public bool IsFeaturedProduct { get; set; }
        public int DisplayOrder { get; set; }
        public int Id { get; set; }

        public SmartstoreManufacturerDto Manufacturer { get; set; }
        public SmartstoreProductDto Product { get; set; }
    }
}
