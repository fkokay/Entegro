using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Customer;
using Entegro.Application.DTOs.Order;
using Entegro.Domain.Enums;
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
            orderDto.OrderSubTotal = smartstoreOrder.OrderSubtotalInclTax;
            orderDto.OrderDiscount = smartstoreOrder.OrderDiscount;
            orderDto.OrderTotal = smartstoreOrder.OrderTotal;
            orderDto.CustomerId = 0;
            orderDto.Deleted = smartstoreOrder.Deleted;
            orderDto.IsTransient = true;
            orderDto.OrderTax = smartstoreOrder.OrderTax;
            orderDto.RefundedAmount = smartstoreOrder.RefundedAmount;
            orderDto.OrderGuid = smartstoreOrder.OrderGuid;
            orderDto.ShippingMethod = smartstoreOrder.ShippingMethod;
            orderDto.PaymentMethod = smartstoreOrder.PaymentMethodSystemName;
            orderDto.PaymentStatus = MapPaymentStatus(smartstoreOrder.PaymentStatusId);
            orderDto.OrderStatus = MapOrderStatus(smartstoreOrder.OrderStatusId);
            orderDto.ShippingStatus = MapShippingStatus(smartstoreOrder.ShippingStatusId);



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

        private static PaymentStatus MapPaymentStatus(int paymentStatusId)
        {
            switch (paymentStatusId)
            {
                case 10: // Pending
                    return PaymentStatus.Pending;

                case 20: // Authorized
                    return PaymentStatus.Authorized;

                case 30: // Paid
                    return PaymentStatus.Paid;

                case 40: // Refunded
                    return PaymentStatus.Refunded;

                case 50: // Voided
                    return PaymentStatus.Voided;

                default: // Bilinmeyen değerler için fallback
                    _logger?.LogWarning("Unknown Smartstore PaymentStatusId: {StatusId}, defaulting to Pending", paymentStatusId);
                    return PaymentStatus.Pending;
            }
        }

        private static OrderStatus MapOrderStatus(int orderStatusId)
        {
            switch (orderStatusId)
            {
                case 10: // Pending (beklemede)
                    return OrderStatus.Pending;

                case 20: // Processing (işleniyor)
                    return OrderStatus.Processing;

                case 30: // Complete (tamamlandı)
                    return OrderStatus.Complete;

                case 40: // Cancelled (iptal edildi)
                    return OrderStatus.Cancelled;

                default: // Tanımlı değilse Pending olarak işaretle
                    _logger?.LogWarning("Unknown Smartstore OrderStatusId: {StatusId}, defaulting to Pending", orderStatusId);
                    return OrderStatus.Pending;
            }
        }

        private static ShippingStatus MapShippingStatus(int shippingStatusId)
        {
            switch (shippingStatusId)
            {
                case 10: // Not yet shipped
                    return ShippingStatus.NotYetShipped;

                case 20: // Partially shipped
                    return ShippingStatus.PartiallyShipped;

                case 30: // Shipped
                    return ShippingStatus.Shipped;

                case 40: // Delivered
                    return ShippingStatus.Delivered;

                default: // Tanımlı değilse NotYetShipped olarak işaretle
                    _logger?.LogWarning("Unknown Smartstore ShippingStatusId: {StatusId}, defaulting to NotYetShipped", shippingStatusId);
                    return ShippingStatus.NotYetShipped;
            }
        }
    }
}
