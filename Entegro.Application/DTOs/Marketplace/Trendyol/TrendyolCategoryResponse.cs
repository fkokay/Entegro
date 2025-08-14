using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.Trendyol
{
    public class TrendyolCategoryResponse
    {
        public List<TrendyolCategoryDto> Categories { get; set; } = new();
    }
}
