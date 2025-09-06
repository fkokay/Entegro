using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities.Checkout
{
    [Table("Invoice")]
    public class Invoice : BaseEntity
    {
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
        // Navigation property
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
    }
}
