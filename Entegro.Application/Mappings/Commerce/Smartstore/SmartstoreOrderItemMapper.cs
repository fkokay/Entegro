using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Order;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Mappings.Commerce.Smartstore
{
    public static class SmartstoreOrderItemMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static OrderItemDto? ToDto(SmartstoreOrderItemDto smartstoreOrderItem)
        {
            try
            {
                if (smartstoreOrderItem == null)
                {
                    return null;
                }

                SmartstoreProductMapper.ConfigureLogger(_logger);

                OrderItemDto orderItem = new OrderItemDto();
                orderItem.Quantity = smartstoreOrderItem.Quantity;
                orderItem.UnitPrice = smartstoreOrderItem.UnitPriceInclTax;
                orderItem.Price = smartstoreOrderItem.PriceInclTax;
                orderItem.DiscountAmount = smartstoreOrderItem.DiscountAmountInclTax;
                orderItem.TaxRate = smartstoreOrderItem.TaxRate;
                orderItem.Product = SmartstoreProductMapper.ToDto(smartstoreOrderItem.Product);


                return orderItem; ;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "OrderItem mapping sırasında hata oluştu. OrderItemId: {OrderItemId}", smartstoreOrderItem.Id);
                return null;
            }
        }

        public static IEnumerable<OrderItemDto> ToDtoList(IEnumerable<SmartstoreOrderItemDto> orderItems)
        {
            if (orderItems == null)
                yield break;

            foreach (var orderItem in orderItems)
            {
                var dto = ToDto(orderItem);
                if (dto != null)
                    yield return dto;
            }
        }
    }
}
