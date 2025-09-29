using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Customer;
using Entegro.Application.DTOs.Order;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Mappings.Commerce.Smartstore
{
    public static class SmartstoreOrderMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static OrderDto? ToDto(SmartstoreOrderDto smartstoreOrder)
        {
            if (smartstoreOrder == null)
            {
                return null;
            }

            SmartstoreOrderItemMapper.ConfigureLogger(_logger);

            OrderDto orderDto = new OrderDto();
            orderDto.OrderNumber = smartstoreOrder.OrderNumber ?? smartstoreOrder.Id.ToString();
            orderDto.OrderDateUtc = smartstoreOrder.CreatedOnUtc;
            orderDto.OrderTotal = smartstoreOrder.OrderTotal;
            orderDto.CustomerId = 0;
            orderDto.Deleted = smartstoreOrder.Deleted;
            orderDto.IsTransient = true;
            orderDto.OrderTax = smartstoreOrder.OrderTax;
            orderDto.OrderDiscount = smartstoreOrder.OrderDiscount;
            orderDto.RefundedAmount = smartstoreOrder.RefundedAmount;
            orderDto.OrderGuid = smartstoreOrder.OrderGuid;
            orderDto.ShippingMethod = smartstoreOrder.ShippingMethod;
            orderDto.PaymentMethod = smartstoreOrder.PaymentMethodSystemName;
            orderDto.PaymentStatus = Domain.Enums.PaymentStatus.Pending;
            orderDto.OrderStatus = Domain.Enums.OrderStatus.Pending;
            orderDto.ShippingStatus = Domain.Enums.ShippingStatus.Shipped;



            orderDto.OrderItems.AddRange(SmartstoreOrderItemMapper.ToDtoList(smartstoreOrder.OrderItems));
            orderDto.OrderNotes.AddRange(SmartstoreOrderNoteMapper.ToDtoList(smartstoreOrder.OrderNotes));

            if (smartstoreOrder.Customer != null)
            {
                SmartstoreCustomerMapper.ConfigureLogger(_logger);
                orderDto.Customer = SmartstoreCustomerMapper.ToDto(smartstoreOrder.Customer);
            }

            SmartstoreAddressMapper.ConfigureLogger(_logger);
            if (smartstoreOrder.ShippingAddress != null)
            {
                orderDto.ShippingAddress = SmartstoreAddressMapper.ToDto(smartstoreOrder.ShippingAddress);
            }

            if (smartstoreOrder.BillingAddress != null)
            {
                orderDto.BillingAddress = SmartstoreAddressMapper.ToDto(smartstoreOrder.BillingAddress);
            }
           

            return orderDto;
        }

        public static IEnumerable<OrderDto> ToDtoList(IEnumerable<SmartstoreOrderDto> orders)
        {
            if (orders == null)
                yield break;

            foreach (var order in orders)
            {
                var dto = ToDto(order);
                if (dto != null)
                    yield return dto;
            }
        }
    }
}
