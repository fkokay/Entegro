namespace Entegro.Web.Models.Dashboard
{
    public class StatisticsViewModel
    {
        public int CustomerCount { get; set; }
        public int ProductCount { get; set; }
        public decimal TotalOrderPrice { get; set; }
        public int CompleteOrderStatusCount { get; set; }

    }
}
