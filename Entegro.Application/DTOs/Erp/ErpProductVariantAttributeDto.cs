using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Erp
{
    public class ErpProductVariantDto
    {
        public string ProductCode { get; set; }
        public string VariantCode { get; set; }
        public string Variant1Name { get; set; }
        public string Variant1Value { get; set; }
        public string Variant2Name { get; set; }
        public string Variant2Value { get; set; }
        public string Variant3Name { get; set; }
        public string Variant3Value { get; set; }
        public string Variant4Name { get; set; }
        public string Variant4Value { get; set; }
        public string Variant5Name { get; set; }
        public string Variant5Value { get; set; }
        public decimal Price { get; set; }
        public decimal StockQuantity { get; set; }
    }
}
