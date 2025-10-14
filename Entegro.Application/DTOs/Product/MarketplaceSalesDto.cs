namespace Entegro.Application.DTOs.Product
{
    public class MarketplaceSalesDto
    {
        public string IntegrationSystemName { get; set; }
        public string IntegrationKey { get; set; }
        public string IntegrationValue { get; set; }
        public int TotalQuantitySold { get; set; }
        public decimal TotalOrderAmount { get; set; }
        public string Period { get; set; }
    }

}
