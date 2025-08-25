using Entegro.Application.DTOs.ProductVariantAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Erp
{
    public class ErpProductDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public string Unit { get; set; }
        public decimal VatRate { get; set; }
        public bool VatInc { get; set; }
        public string BrandName { get; set; }
        public string GroupName { get; set; }
        public string Barcode { get; set; }
        public string Category1 { get; set; }
        public string Category2 { get; set; }
        public string Category3 { get; set; }
        public string Category4 { get; set; }

        public List<ErpProductVariantDto> ProductVariantAttributes { get; set; } = new List<ErpProductVariantDto>();
    }
}
