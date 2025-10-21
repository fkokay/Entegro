namespace Entegro.Web.Models.Checkout.Orders
{
    public class OrderInvoiceModel
    {
        public string? CustomerName { get; set; }
        public string? BillingAddress { get; set; }
        public string? ShippingAddress { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}
