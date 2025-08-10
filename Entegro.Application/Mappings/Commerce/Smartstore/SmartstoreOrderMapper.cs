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
            orderDto.OrderNo = smartstoreOrder.OrderNumber ?? smartstoreOrder.Id.ToString();
            orderDto.OrderDate = smartstoreOrder.CreatedOnUtc;
            orderDto.TotalAmount = smartstoreOrder.OrderTotal;
            orderDto.OrderSource = Domain.Enums.OrderSource.Smartstore;
            orderDto.CustomerId = 0;
            orderDto.Deleted = smartstoreOrder.Deleted;
            orderDto.IsTransient = true;

            orderDto.OrderItems.AddRange(SmartstoreOrderItemMapper.ToDtoList(smartstoreOrder.OrderItems));
            if(smartstoreOrder.Customer != null)
            {
                SmartstoreCustomerMapper.ConfigureLogger(_logger);
                orderDto.Customer = SmartstoreCustomerMapper.ToDto(smartstoreOrder.Customer);
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
