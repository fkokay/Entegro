using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public bool Deleted { get; set; }
        public bool IsTransient { get; set; }
    }
}
