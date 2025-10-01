namespace Entegro.Application.DTOs.OrderNote
{
    public class OrderNoteDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Note { get; set; }
        public DateTime CreatedOn { get; set; }
    }

}
