namespace Entegro.Application.DTOs.Product
{
    public class ProductSalesDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string IntegrationSystemName { get; set; }
        public string IntegrationKey { get; set; }
        public string IntegrationValue { get; set; }
        public int TotalQuantitySold { get; set; }
        public string Period { get; set; }
    }

}
