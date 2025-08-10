using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.Trendyol
{
    public class TrendyolProductAttributeDto
    {
        public int attributeId { get; set; }
        public string attributeName { get; set; }
        public int? attributeValueId { get; set; }
        public string attributeValue { get; set; }
    }
}
