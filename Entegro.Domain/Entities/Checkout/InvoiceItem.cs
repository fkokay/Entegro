using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Entities.Checkout
{
    [Table("InvoiceItem")]
    public class InvoiceItem : BaseEntity
    {
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        // Navigation property
        public int InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }
    }
}
