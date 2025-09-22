namespace Entegro.Web.Models.Checkout.Orders
{
    public class OrderNoteModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Note { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}
