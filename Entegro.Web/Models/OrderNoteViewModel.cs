namespace Entegro.Web.Models
{
    public class OrderNoteViewModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Note { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}
