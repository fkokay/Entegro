using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Commerce.Smartstore
{
    public class SmartstoreOrderNoteDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Note { get; set; } = string.Empty;
        public bool DisplayToCustomer { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}
