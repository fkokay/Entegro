using Entegro.Application.DTOs.Marketplace.Pazarama;
using Entegro.Application.DTOs.OrderItem;
using Microsoft.Extensions.Logging;

namespace Entegro.Application.Mappings.Marketplace.Pazarama
{
    public class PazaramaOrderLineMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static OrderItemDto? ToDto(OrderItem pazaramaOrder)
        {
            if (pazaramaOrder == null)
            {
                return null;
            }

            OrderItemDto orderItem = new OrderItemDto();
            orderItem.Quantity = pazaramaOrder.Quantity;
            orderItem.DiscountAmount = Convert.ToDecimal(pazaramaOrder.DiscountAmount.Value);
            orderItem.Price = Convert.ToDecimal(pazaramaOrder.TotalPrice.Value);
            orderItem.UnitPrice = Convert.ToDecimal(pazaramaOrder.SalePrice.Value);
            orderItem.Product = new DTOs.Product.ProductDto();
            orderItem.Product.Code = pazaramaOrder.Product.Code == null ? pazaramaOrder.Product.Barcode : pazaramaOrder.Product.Code;
            orderItem.TaxRate = Convert.ToDecimal(pazaramaOrder.TaxAmount.Value);
            orderItem.Sku = pazaramaOrder.Product.Code == null ? pazaramaOrder.Product.Barcode : pazaramaOrder.Product.Code;
            orderItem.IntegrationSku = pazaramaOrder.Product.Code == null ? pazaramaOrder.Product.Barcode : pazaramaOrder.Product.Code;
            orderItem.IntegrationProductName = pazaramaOrder.Product.Name;
            orderItem.ProductCost = 0;
            orderItem.ItemWeight = 0;
            orderItem.AttributesXml = "";
            orderItem.IntegrationProductImageUrl = pazaramaOrder.Product.ImageUrl;


            return orderItem;
        }

        public static IEnumerable<OrderItemDto> ToDtoList(IEnumerable<OrderItem> orderLines)
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
