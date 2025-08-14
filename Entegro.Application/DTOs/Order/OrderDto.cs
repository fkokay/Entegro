using Entegro.Application.DTOs.Customer;
using Entegro.Application.DTOs.OrderItem;
using Entegro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int OrderSourceId { get; set; }
        [NotMapped]
        public OrderSource OrderSource
        {
            get => (OrderSource)OrderSourceId;
            set => OrderSourceId = (int)value;
        }
        [NotMapped]
        public string OrderSourceLabelHint
        {
            get
            {
                return OrderSource switch
                {
                    OrderSource.Smartstore => "Smartstore",
                    OrderSource.Trendyol => "Trendyol"
                };
            }
        }
        public string OrderNo { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public bool Deleted { get; set; }
        public bool IsTransient { get; set; }

        public List<OrderItemDto> OrderItems { get; set; } = new();
        public CustomerDto? Customer { get; set; }
    }
}
