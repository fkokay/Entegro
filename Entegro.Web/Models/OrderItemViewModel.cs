namespace Entegro.Web.Models
{
    public class OrderItemViewModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Price { get; set; }
        public decimal TaxRate { get; set; }
        public decimal DiscountAmount { get; set; }

        public OrderViewModel? Order { get; set; }
        public ProductViewModel? Product { get; set; }
    }
}
