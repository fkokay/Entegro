using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Marketplace.Trendyol
{
    public class TrendyolDiscountDetailDto
    {
        public decimal LineItemPrice { get; set; }
        public decimal LineItemDiscount { get; set; }
        public decimal LineItemTyDiscount { get; set; }
    }
}
