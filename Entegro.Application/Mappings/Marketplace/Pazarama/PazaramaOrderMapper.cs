using Entegro.Application.DTOs.Marketplace.Pazarama;
using Entegro.Application.DTOs.Order;
using Entegro.Application.DTOs.Shipment;
using Entegro.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Entegro.Application.Mappings.Marketplace.Pazarama
{
    public static class PazaramaOrderMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static OrderDto? ToDto(OrderData pazaramaOrder)
        {
            if (pazaramaOrder == null)
            {
                return null;
            }

            OrderDto order = new OrderDto();
            order.OrderNumber = pazaramaOrder.OrderNumber.ToString();
            order.OrderGuid = Guid.NewGuid();
            order.OrderTax = 0;
            order.RefundedAmount = 0;
            order.OrderTotal = Convert.ToDecimal(pazaramaOrder.OrderAmount);
            order.PaymentMethod = "Pazarama";
            order.OrderDateUtc = Convert.ToDateTime(pazaramaOrder.OrderDate);
            order.Deleted = false;
            order.IsTransient = true;
            order.OrderDiscount = Convert.ToDecimal(pazaramaOrder.DiscountAmount);//ekledim
            order.OrderSubTotal = 0;//ekledim
            order.InvoiceLink = "";//ekledim
            var i = pazaramaOrder.OrderStatus;
            order.OrderStatus = 0;
            order.PaymentStatus = pazaramaOrder.PaymentType == 1 ? PaymentStatus.Paid : PaymentStatus.Pending;
            order.ShippingMethod = "";


            order.DueDateUtc = DateTime.UtcNow;
            order.Customer = new DTOs.Customer.CustomerDto()
            {
                Address = pazaramaOrder.ShipmentAddress.AddressDetail,
                City = pazaramaOrder.ShipmentAddress.CityName,
                Email = pazaramaOrder.CustomerEmail,
                CustomerType = 1,
                District = pazaramaOrder.ShipmentAddress.DistrictName,
                TaxNumber = "",
                PhoneNumber = pazaramaOrder.ShipmentAddress.PhoneNumber,
                Street = pazaramaOrder.ShipmentAddress.NeighborhoodName,
                TaxOffice = "",
                Town = "",
                Name = pazaramaOrder.CustomerName,
            };
            order.BillingAddress = new DTOs.Address.AddressDto()
            {
                Address1 = pazaramaOrder.BillingAddress.AddressDetail,
                Address2 = pazaramaOrder.BillingAddress.AddressDetail,
                AddressType = "Fatura Adresi",
                City = pazaramaOrder.BillingAddress.CityName,
                Company = pazaramaOrder.BillingAddress.AddressDetail,
                Country = "",
                District = pazaramaOrder.BillingAddress.DistrictName,
                Email = pazaramaOrder.BillingAddress.CustomerEmail,
                FaxNumber = "",
                FirstName = pazaramaOrder.ShipmentAddress.NameSurname,
                LastName = "",
                PhoneNumber = "",
                Salutation = "",
                TaxOffice = "",
                TaxOfficeNumber = "",
                Town = "",
                Title = "",
                ZipPostalCode = "",
            };
            order.ShippingAddress = new DTOs.Address.AddressDto()
            {
                Address1 = pazaramaOrder.ShipmentAddress.AddressDetail,
                Address2 = pazaramaOrder.ShipmentAddress.AddressDetail,
                AddressType = "Teslimat Adresi",
                City = pazaramaOrder.ShipmentAddress.CityName,
                Company = "",
                Country = "",
                District = pazaramaOrder.ShipmentAddress.DistrictName,
                Email = pazaramaOrder.ShipmentAddress.CustomerEmail,
                FaxNumber = "",
                FirstName = pazaramaOrder.ShipmentAddress.NameSurname,
                LastName = "",
                PhoneNumber = "",
                Salutation = "",
                TaxOffice = "",
                TaxOfficeNumber = "",
                Town = "",
                Title = "",
                ZipPostalCode = "",
            };
            order.OrderItems = new List<DTOs.OrderItem.OrderItemDto>();

            ShipmentDto shipmentDto = new ShipmentDto();
            shipmentDto.OrderId = 0;
            shipmentDto.Carrier = pazaramaOrder.Items.Select(x => x.Cargo.CompanyName).FirstOrDefault();
            shipmentDto.PackageNo = pazaramaOrder.Items.Select(x => x.Cargo.CompanyId).FirstOrDefault();
            shipmentDto.TrackingNumber = pazaramaOrder.Items.Select(x => x.Cargo.TrackingNumber).FirstOrDefault();
            shipmentDto.TrackingUrl = pazaramaOrder.Items.Select(x => x.Cargo.TrackingUrl).FirstOrDefault();
            shipmentDto.TotalWeight = 0;
            shipmentDto.ShippedDate = null;
            shipmentDto.DeliveryDate = null;
            shipmentDto.CreatedOn = DateTime.UtcNow;

            order.Shipments.Add(shipmentDto);


            return order;

        }

        public static IEnumerable<OrderDto> ToDtoList(IEnumerable<OrderData> pazaramaOrders)
        {
            if (pazaramaOrders == null)
                yield break;

            foreach (var pazaramaOrder in pazaramaOrders)
            {
                var dto = ToDto(pazaramaOrder);
                if (dto != null)
                    yield return dto;
            }
        }
    }
}
