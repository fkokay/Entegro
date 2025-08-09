using Entegro.Application.DTOs.Customer;
using Entegro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Order
{
    public class UpdateOrderDto
    {
        public int Id { get; set; }
        public OrderSource OrderSource { get; set; }
        public string OrderNo { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public bool Deleted { get; set; }
        public bool IsTransient { get; set; }

        public List<CreateOrderItemDto> OrderItems { get; set; } = new();
        public CustomerDto Customer { get; set; }
    }
}
