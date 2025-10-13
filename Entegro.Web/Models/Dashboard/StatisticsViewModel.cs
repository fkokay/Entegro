using Entegro.Web.Models.Catalog.Products;

namespace Entegro.Web.Models.Dashboard
{
    public class StatisticsViewModel
    {
        public int CustomerCount { get; set; }
        public int ProductCount { get; set; }
        public decimal TotalOrderPrice { get; set; }
        public int CompleteOrderStatusCount { get; set; }

    }
    public class ViewSalesViewModel
    {
        public decimal TotalOrderPrice { get; set; }

    }
    public class GeneratedLeadsViewModel
    {
        public int CurrentMonthCustomerCount { get; set; }

    }
    public class PopularProductViewModel
    {
        public List<ProductModel> Products { get; set; } = new List<ProductModel>();

    }

    public class ProductSalesViewModel
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
