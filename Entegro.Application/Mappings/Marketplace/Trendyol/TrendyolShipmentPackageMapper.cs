using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.DTOs.Marketplace.Trendyol;
using Entegro.Application.DTOs.Order;
using Entegro.Application.Mappings.Commerce.Smartstore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Mappings.Marketplace.Trendyol
{
    public class TrendyolShipmentPackageMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static OrderDto? ToDto(TrendyolShipmentPackageDto trendyolShipmentPackage)
        {
            if (trendyolShipmentPackage == null)
            {
                return null;
            }

            TrendyolOrderLineMapper.ConfigureLogger(_logger);

            OrderDto order = new OrderDto();
            order.OrderNo = trendyolShipmentPackage.OrderNumber;
            order.OrderDate = FromUnixTimeMilliseconds(trendyolShipmentPackage.OrderDate);
            order.TotalAmount = trendyolShipmentPackage.TotalPrice;
            order.OrderSource = Domain.Enums.OrderSource.Trendyol;
            order.CustomerId = 0;
            order.Deleted = false;
            order.IsTransient = true;

            order.OrderItems = TrendyolOrderLineMapper.ToDtoList(trendyolShipmentPackage.Lines).ToList();
            order.Customer = new DTOs.Customer.CustomerDto();
            order.Customer.Name = trendyolShipmentPackage.CustomerFirstName + " " + trendyolShipmentPackage.CustomerLastName;
            order.Customer.Email = trendyolShipmentPackage.CustomerEmail;
            order.Customer.Address = trendyolShipmentPackage.InvoiceAddress.Address1 + " " + trendyolShipmentPackage.InvoiceAddress.Address2;
            order.Customer.TaxNumber = trendyolShipmentPackage.TaxNumber;
            order.Customer.TaxOffice = trendyolShipmentPackage.InvoiceAddress.TaxOffice;
            order.Customer.CustomerType = trendyolShipmentPackage.IdentityNumber != null ? 1 : 0;
            order.Customer.PhoneNumber = trendyolShipmentPackage.InvoiceAddress.Phone;
            order.Customer.CreatedOn = DateTime.Now;
            order.Customer.UpdatedOn = DateTime.Now;


            return order;
        }

        public static IEnumerable<OrderDto> ToDtoList(IEnumerable<TrendyolShipmentPackageDto> orders)
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

        public static DateTime FromUnixTimeMilliseconds(long milliseconds)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
            return dateTimeOffset.UtcDateTime;
        }
    }
}
