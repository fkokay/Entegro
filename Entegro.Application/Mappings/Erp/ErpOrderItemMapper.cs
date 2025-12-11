using Entegro.Application.DTOs.Erp;
using Entegro.Application.DTOs.OrderItem;
using Microsoft.Extensions.Logging;

namespace Entegro.Application.Mappings.Erp
{
    internal class ErpOrderItemMapper
    {

        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static OrderItemDto? ToDto(ErpOrderItemDto erpOrderItem)
        {
            if (erpOrderItem == null)
            {
                return null;
            }

            OrderItemDto orderItem = new OrderItemDto();
            orderItem.Quantity = erpOrderItem.Quantity;
            orderItem.DiscountAmount = erpOrderItem.DiscountAmount;
            orderItem.Price = erpOrderItem.Price;
            orderItem.UnitPrice = erpOrderItem.UnitPrice;
            orderItem.TaxRate = erpOrderItem.VatRate;
            orderItem.Product = new DTOs.Product.ProductDto();
            orderItem.Product.Code = erpOrderItem.ProductCode;
            orderItem.Sku = erpOrderItem.ProductCode;
            orderItem.IntegrationSku = erpOrderItem.ProductCode;
            orderItem.IntegrationProductName = erpOrderItem.ProductName;
            orderItem.ProductCost = 0;
            orderItem.ItemWeight = 0;
            orderItem.AttributesXml = "";

            return orderItem;
        }

        public static IEnumerable<OrderItemDto> ToDtoList(IEnumerable<ErpOrderItemDto> orderItems)
        {
            if (orderItems == null)
                yield break;

            foreach (var orderLine in orderItems)
            {
                var dto = ToDto(orderLine);
                if (dto != null)
                    yield return dto;
            }
        }
    }
}
