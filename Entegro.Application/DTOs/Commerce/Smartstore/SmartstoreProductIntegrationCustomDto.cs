namespace Entegro.Application.DTOs.Commerce.Smartstore
{
    public class SmartstoreProductIntegrationCustomDto
    {
        public int ManageInventoryMethod { get; set; } = 2;
        public int StockQunatity { get; set; }
        public bool DisplayStockAvailability { get; set; } = true;
        public bool DisplayStockQuantity { get; set; } = true;
        public int MinStockQuantity { get; set; }= 0;
        public int LowStockActivityId { get; set; } = 0;
        public bool ShowOnHomePage { get; set; }
        public int HomePageDisplayOrder { get; set; }
        public decimal? SpecialPrice { get; set; }
        public DateTime? SpecialPriceStartDateTime { get; set; }
        public DateTime? SpecialPriceEndDateTime { get; set; }

    }
}
