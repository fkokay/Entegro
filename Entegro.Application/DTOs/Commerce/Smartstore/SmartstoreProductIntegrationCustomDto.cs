using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Commerce.Smartstore
{
    public class SmartstoreProductIntegrationCustomDto
    {
        public int ManageInventoryMethod { get; set; }
        public int StockQunatity { get; set; }
        public bool DisplayStockAvailability { get; set; }
        public bool DisplayStockQuantity { get; set; }
        public int MinStockQuantity { get; set; }
        public int LowStockActivityId { get; set; }
        public bool ShowOnHomePage { get; set; }
        public int HomePageDisplayOrder { get; set; }
        public decimal? SpecialPrice { get; set; }
        public DateTime? SpecialPriceStartDateTime { get; set; }
        public DateTime? SpecialPriceEndDateTime { get; set; }
    }
}
