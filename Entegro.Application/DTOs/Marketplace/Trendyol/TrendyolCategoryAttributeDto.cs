using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.Trendyol
{
    public class TrendyolCategoryAttributeDto
    {
        public int CategoryId { get; set; }
        public TrendyolAttributeDetailDto Attribute { get; set; } = new();
        public bool Required { get; set; }
        public bool AllowCustom { get; set; }
        public bool Varianter { get; set; }
        public bool Slicer { get; set; }
        public List<TrendyolCategoryAttributeValueDto> AttributeValues { get; set; } = new();
    }
}
