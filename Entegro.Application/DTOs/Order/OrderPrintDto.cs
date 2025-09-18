using Entegro.Application.DTOs.Address;
using Entegro.Application.DTOs.IntegrationSystem;
using Entegro.Application.DTOs.OrderItem;
using Entegro.Application.DTOs.Shipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.DTOs.Order
{
    public class OrderPrintDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public ShipmentDto Shipment { get; set; }
        public AddressDto? ShippingAddress { get; set; }
        public AddressDto? BillingAddress { get;set; }
        public IntegrationSystemDto IntegrationSystem { get; set; }

        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }
}
