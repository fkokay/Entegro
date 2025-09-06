namespace Entegro.Application.DTOs.OrderNote
{
    public class UpdateOrderNoteDto
    {

        public int OrderId { get; set; }
        public string Note { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }

}
