using Entegro.Application.DTOs.Marketplace.Trendyol;
using Entegro.Application.DTOs.Order;
using Entegro.Application.DTOs.OrderItem;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Mappings.Marketplace.Trendyol
{
    public class TrendyolOrderLineMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static OrderItemDto? ToDto(TrendyolOrderLineDto trendyolOrderLine)
        {
            if (trendyolOrderLine == null)
            {
                return null;
            }


            OrderItemDto orderItem = new OrderItemDto();
            orderItem.Quantity = trendyolOrderLine.Quantity;
            orderItem.DiscountAmount = trendyolOrderLine.Discount;
            orderItem.Price = trendyolOrderLine.Price;
            orderItem.UnitPrice = trendyolOrderLine.Amount;
            orderItem.TaxRate = trendyolOrderLine.VatBaseAmount;
            orderItem.Product = new DTOs.Product.ProductDto();
            orderItem.Product.Code = trendyolOrderLine.ProductCode.ToString();
            orderItem.TaxRate = trendyolOrderLine.VatBaseAmount;


            return orderItem;
        }

        public static IEnumerable<OrderItemDto> ToDtoList(IEnumerable<TrendyolOrderLineDto> orderLines)
        {
            if (orderLines == null)
                yield break;

            foreach (var orderLine in orderLines)
            {
                var dto = ToDto(orderLine);
                if (dto != null)
                    yield return dto;
            }
        }
    }
}
