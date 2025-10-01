using Entegro.Application.DTOs.Order;
using Entegro.Domain.Entities.Checkout;

namespace Entegro.Application.DTOs.Invoice
{
    public class InvoiceDto
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public string Currency { get; set; }
        public string BillingAddress { get; set; }
        public string ShippingAddress { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string Status { get; set; }
        public int OrderId { get; set; }
        public OrderDto Order { get; set; }
        public List<InvoiceItem> InvoiceItems { get; set; } = new();
    }
}
