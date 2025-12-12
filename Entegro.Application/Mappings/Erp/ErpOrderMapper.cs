using Entegro.Application.DTOs.Erp;
using Entegro.Application.DTOs.Order;
using Entegro.Application.DTOs.Shipment;
using Entegro.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Entegro.Application.Mappings.Erp
{
    public class ErpOrderMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static OrderDto? ToDto(ErpOrderDto erpOrder)
        {
            if (erpOrder == null)
            {
                return null;
            }

            OrderDto order = new OrderDto();
            order.OrderNumber = erpOrder.OrderNumber;
            order.OrderGuid = Guid.NewGuid();
            order.OrderTax = erpOrder.OrderTax;
            order.RefundedAmount = erpOrder.RefundedAmount;
            order.OrderTotal = erpOrder.OrderTotal;
            order.PaymentMethod = "ErpLogo";
            order.OrderDateUtc = erpOrder.OrderDate;
            order.Deleted = false;
            order.IsTransient = true;
            order.OrderDiscount = erpOrder.OrderDiscount;
            order.OrderSubTotal = erpOrder.OrderSubTotal;
            order.InvoiceLink = "";//ekledim

            order.OrderStatus = OrderStatus.Pending;
            order.ShippingStatus = ShippingStatus.ShippingNotRequired;
            order.PaymentStatus = PaymentStatus.Paid;
            order.ShippingMethod = erpOrder.ShippingMethod;

            order.OrderItems = ErpOrderItemMapper.ToDtoList(erpOrder.OrderItems).ToList();
            order.DueDateUtc = DateTime.MinValue;
            order.Customer = new DTOs.Customer.CustomerDto()
            {
                Address = erpOrder.InvoiceAddress.Address1,
                City = erpOrder.InvoiceAddress.City,
                Email = string.Empty,
                CustomerType = 1,
                District = erpOrder.InvoiceAddress.District,
                TaxNumber = erpOrder.InvoiceAddress.TaxOfficeNumber,
                PhoneNumber = erpOrder.InvoiceAddress.PhoneNumber,
                Street = "",
                TaxOffice = erpOrder.InvoiceAddress.TaxOffice,
                Town = erpOrder.InvoiceAddress.Town,
                Name = erpOrder.CustomerName,
            };
            order.BillingAddress = new DTOs.Address.AddressDto()
            {
                Address1 = erpOrder.InvoiceAddress.Address1,
                Address2 = erpOrder.InvoiceAddress.Address2,
                AddressType = "Fatura Adresi",
                City = erpOrder.InvoiceAddress.City,
                Company = erpOrder.InvoiceAddress.Company,
                Country = erpOrder.InvoiceAddress.Country,
                District = erpOrder.InvoiceAddress.District,
                Email = erpOrder.InvoiceAddress.Email,
                FaxNumber = erpOrder.InvoiceAddress.FaxNumber,
                FirstName = erpOrder.InvoiceAddress.FirstName,
                LastName = erpOrder.InvoiceAddress.LastName,
                PhoneNumber = erpOrder.InvoiceAddress.PhoneNumber,
                Salutation = erpOrder.InvoiceAddress.Salutation,
                TaxOffice = erpOrder.InvoiceAddress.TaxOffice,
                TaxOfficeNumber = erpOrder.InvoiceAddress.TaxOfficeNumber,
                Town = erpOrder.InvoiceAddress.Town,
                Title = erpOrder.InvoiceAddress.Title,
                ZipPostalCode = erpOrder.InvoiceAddress.ZipPostalCode,
            };
            order.ShippingAddress = new DTOs.Address.AddressDto()
            {
                Address1 = erpOrder.ShippingAddress.Address1,
                Address2 = erpOrder.ShippingAddress.Address2,
                AddressType = "Teslimat Adresi",
                City = erpOrder.ShippingAddress.City,
                Company = erpOrder.ShippingAddress.Company,
                Country = erpOrder.ShippingAddress.Country,
                District = erpOrder.ShippingAddress.District,
                Email = erpOrder.ShippingAddress.Email,
                FaxNumber = erpOrder.ShippingAddress.FaxNumber,
                FirstName = erpOrder.ShippingAddress.FirstName,
                LastName = erpOrder.ShippingAddress.LastName,
                PhoneNumber = erpOrder.ShippingAddress.PhoneNumber,
                Salutation = erpOrder.ShippingAddress.Salutation,
                TaxOffice = erpOrder.ShippingAddress.TaxOffice,
                TaxOfficeNumber = erpOrder.ShippingAddress.TaxOfficeNumber,
                Town = erpOrder.ShippingAddress.Town,
                Title = erpOrder.ShippingAddress.Title,
                ZipPostalCode = erpOrder.ShippingAddress.ZipPostalCode,
            };

            ShipmentDto shipmentDto = new ShipmentDto();
            shipmentDto.OrderId = 0;
            shipmentDto.Carrier = erpOrder.ShippingMethod;
            shipmentDto.PackageNo = "";
            shipmentDto.TrackingNumber = "";
            shipmentDto.TrackingUrl = "";
            shipmentDto.TotalWeight = 1;
            shipmentDto.ShippedDate = null;
            shipmentDto.DeliveryDate = null;
            shipmentDto.CreatedOn = DateTime.UtcNow;

            order.Shipments.Add(shipmentDto);
            return order;
        }

        public static IEnumerable<OrderDto> ToDtoList(IEnumerable<ErpOrderDto> erpOrders)
        {
            if (erpOrders == null)
                yield break;

            foreach (var order in erpOrders)
            {
                var dto = ToDto(order);
                if (dto != null)
                    yield return dto;
            }
        }
    }

}
