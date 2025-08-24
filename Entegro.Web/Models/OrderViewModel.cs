using Entegro.Domain.Enums;

namespace Entegro.Web.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public int OrderSourceId { get; set; }

        public OrderSource OrderSource
        {
            get => (OrderSource)OrderSourceId;
            set => OrderSourceId = (int)value;
        }

        public string OrderSourceLabelHint
        {
            get
            {
                return OrderSource switch
                {
                    OrderSource.Smartstore => "Smartstore",
                    OrderSource.Trendyol => "Trendyol"
                };
            }
        }
        public string OrderNo { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public bool Deleted { get; set; }
        public bool IsTransient { get; set; }

        public List<OrderItemViewModel> OrderItems { get; set; } = new();
        public CustomerViewModel? Customer { get; set; }
    }
}
