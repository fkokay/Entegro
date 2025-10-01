namespace Entegro.Application.DTOs.Invoice
{
    public class InvoiceItemDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public int InvoiceId { get; set; }
        public virtual InvoiceDto Invoice { get; set; }
    }
}
