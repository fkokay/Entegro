using Entegro.Application.DTOs.Marketplace.Trendyol;
using Entegro.Application.DTOs.OrderItem;
using Microsoft.Extensions.Logging;

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
            orderItem.Product.Code = trendyolOrderLine.Barcode;
            orderItem.TaxRate = trendyolOrderLine.VatBaseAmount;
            orderItem.Sku = trendyolOrderLine.Sku;
            orderItem.IntegrationSku = trendyolOrderLine.Sku;
            orderItem.IntegrationProductName = trendyolOrderLine.ProductName;
            orderItem.ProductCost = 0;
            orderItem.ItemWeight = 0;
            orderItem.AttributesXml = "";


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
