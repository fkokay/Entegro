namespace Entegro.Web.Models.Checkout.Orders
{
    public class OrderPackageViewModel
    {
        public int OrderId { get; set; }
        public int OrderItemId { get; set; }
        public int Quantity { get; set; }
        public bool IsPackage { get; set; }
    }
}
