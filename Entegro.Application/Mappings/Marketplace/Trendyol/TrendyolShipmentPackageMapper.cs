using Entegro.Application.DTOs.Marketplace.Trendyol;
using Entegro.Application.DTOs.Order;
using Entegro.Application.DTOs.Shipment;
using Entegro.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Entegro.Application.Mappings.Marketplace.Trendyol
{
    public class TrendyolShipmentPackageMapper
    {
        private static ILogger _logger;
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
            order.OrderNumber = trendyolShipmentPackage.OrderNumber;
            order.OrderGuid = Guid.NewGuid();
            order.OrderTax = 0;
            order.RefundedAmount = 0;
            order.OrderTotal = trendyolShipmentPackage.TotalPrice;
            order.PaymentMethod = "Trendyol";
            order.OrderDateUtc = FromUnixTimeMilliseconds(trendyolShipmentPackage.OrderDate);
            order.Deleted = false;
            order.IsTransient = true;
            order.OrderDiscount = trendyolShipmentPackage.TotalDiscount;//ekledim
            order.OrderSubTotal = trendyolShipmentPackage.GrossAmount;//ekledim
            order.InvoiceLink = trendyolShipmentPackage.InvoiceLink;//ekledim

            order.OrderStatus = TrendyolStatusMapper.MapOrderStatus(trendyolShipmentPackage.Status);
            order.PaymentStatus = TrendyolStatusMapper.MapPaymentStatus(trendyolShipmentPackage.Status);
            order.ShippingMethod = trendyolShipmentPackage.CargoProviderName;
            order.ShippingStatus = TrendyolStatusMapper.MapShippingStatus(trendyolShipmentPackage.ShipmentPackageStatus);


            order.DueDateUtc = FromUnixTimeMilliseconds(trendyolShipmentPackage.AgreedDeliveryDate);
            order.Customer = new DTOs.Customer.CustomerDto()
            {
                Address = trendyolShipmentPackage.InvoiceAddress.Address1,
                City = trendyolShipmentPackage.InvoiceAddress.City,
                Email = trendyolShipmentPackage.CustomerEmail,
                CustomerType = 1,
                District = trendyolShipmentPackage.InvoiceAddress.District,
                TaxNumber = trendyolShipmentPackage.TaxNumber,
                PhoneNumber = "",
                Street = "",
                TaxOffice = trendyolShipmentPackage.InvoiceAddress.TaxOffice,
                Town = "",
                Name = trendyolShipmentPackage.CustomerFirstName + " " + trendyolShipmentPackage.CustomerLastName,
            };
            order.BillingAddress = new DTOs.Address.AddressDto()
            {
                Address1 = trendyolShipmentPackage.InvoiceAddress.Address1,
                Address2 = trendyolShipmentPackage.InvoiceAddress.Address2,
                AddressType = "Fatura Adresi",
                City = trendyolShipmentPackage.InvoiceAddress.City,
                Company = trendyolShipmentPackage.InvoiceAddress.Company,
                Country = trendyolShipmentPackage.InvoiceAddress.CountyName,
                District = trendyolShipmentPackage.InvoiceAddress.District,
                Email = trendyolShipmentPackage.CustomerEmail,
                FaxNumber = "",
                FirstName = trendyolShipmentPackage.InvoiceAddress.FirstName,
                LastName = trendyolShipmentPackage.InvoiceAddress.LastName,
                PhoneNumber = "",
                Salutation = "",
                TaxOffice = trendyolShipmentPackage.InvoiceAddress.TaxOffice,
                TaxOfficeNumber = trendyolShipmentPackage.InvoiceAddress.TaxNumber,
                Town = "",
                Title = "",
                ZipPostalCode = trendyolShipmentPackage.InvoiceAddress.PostalCode,
            };
            order.ShippingAddress = new DTOs.Address.AddressDto()
            {
                Address1 = trendyolShipmentPackage.ShipmentAddress.Address1,
                Address2 = trendyolShipmentPackage.ShipmentAddress.Address2,
                AddressType = "Teslimat Adresi",
                City = trendyolShipmentPackage.ShipmentAddress.City,
                Company = trendyolShipmentPackage.ShipmentAddress.Company,
                Country = trendyolShipmentPackage.ShipmentAddress.CountyName,
                District = trendyolShipmentPackage.ShipmentAddress.District,
                Email = trendyolShipmentPackage.CustomerEmail,
                FaxNumber = "",
                FirstName = trendyolShipmentPackage.ShipmentAddress.FirstName,
                LastName = trendyolShipmentPackage.ShipmentAddress.LastName,
                PhoneNumber = "",
                Salutation = "",
                TaxOffice = trendyolShipmentPackage.ShipmentAddress.TaxOffice,
                TaxOfficeNumber = trendyolShipmentPackage.ShipmentAddress.TaxNumber,
                Town = "",
                Title = "",
                ZipPostalCode = trendyolShipmentPackage.ShipmentAddress.PostalCode,
            };
            order.OrderItems = TrendyolOrderLineMapper.ToDtoList(trendyolShipmentPackage.Lines).ToList();

            ShipmentDto shipmentDto = new ShipmentDto();
            shipmentDto.OrderId = 0;
            shipmentDto.Carrier = trendyolShipmentPackage.CargoProviderName;
            shipmentDto.PackageNo = trendyolShipmentPackage.Id.ToString();
            shipmentDto.TrackingNumber = trendyolShipmentPackage.CargoTrackingNumber.ToString();
            shipmentDto.TrackingUrl = trendyolShipmentPackage.CargoTrackingLink;
            shipmentDto.TotalWeight = trendyolShipmentPackage.CargoDeci;
            shipmentDto.ShippedDate = null;
            shipmentDto.DeliveryDate = null;
            shipmentDto.CreatedOn = DateTime.UtcNow;

            order.Shipments.Add(shipmentDto);


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
