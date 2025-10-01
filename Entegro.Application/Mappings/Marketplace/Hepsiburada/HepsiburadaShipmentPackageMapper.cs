using Entegro.Application.DTOs.Marketplace.Hepsiburada;
using Entegro.Application.DTOs.Order;
using Entegro.Application.DTOs.Shipment;
using Microsoft.Extensions.Logging;

namespace Entegro.Application.Mappings.Marketplace.Hepsiburada
{
    public static class HepsiburadaShipmentPackageMapper
    {
        private static ILogger? _logger;
        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static OrderDto? ToDto(HepsiburadaShipmentPackageDto hepsiburadaShipmentPackage)
        {
            if (hepsiburadaShipmentPackage == null)
            {
                return null;
            }

            OrderDto order = new OrderDto();
            order.OrderNumber = hepsiburadaShipmentPackage.Id;
            order.OrderGuid = Guid.Parse(hepsiburadaShipmentPackage.Id);
            order.OrderTax = 0;
            order.OrderDiscount = 0;
            order.RefundedAmount = 0;
            order.OrderTotal = hepsiburadaShipmentPackage.TotalPrice.Amount;
            order.PaymentMethod = "Hepsiburada";
            order.OrderDateUtc = hepsiburadaShipmentPackage.OrderDate;
            order.Deleted = false;
            order.IsTransient = false;
            order.OrderStatus = Domain.Enums.OrderStatus.Pending;
            order.PaymentStatus = Domain.Enums.PaymentStatus.Paid;
            order.ShippingMethod = hepsiburadaShipmentPackage.CargoCompany;
            order.ShippingStatus = Domain.Enums.ShippingStatus.Shipped;
            order.DueDateUtc = hepsiburadaShipmentPackage.DueDate;
            order.Customer = new DTOs.Customer.CustomerDto()
            {
                Address = hepsiburadaShipmentPackage.BillingAddress,
                City = hepsiburadaShipmentPackage.BillingCity,
                Email = hepsiburadaShipmentPackage.Email,
                CustomerType = 1,
                District = hepsiburadaShipmentPackage.BillingDistrict,
                TaxNumber = hepsiburadaShipmentPackage.TaxNumber + hepsiburadaShipmentPackage.IdentityNo,
                PhoneNumber = "",
                Street = "",
                TaxOffice = "",
                Town = hepsiburadaShipmentPackage.BillingTown,
                Name = hepsiburadaShipmentPackage.CustomerName,
            };
            order.BillingAddress = new DTOs.Address.AddressDto()
            {
                Address1 = hepsiburadaShipmentPackage.BillingAddress,
                Address2 = "",
                AddressType = "Fatura Adresi",
                City = "",
                Company = hepsiburadaShipmentPackage.CompanyName,
                Country = "",
                District = "",
                Email = hepsiburadaShipmentPackage.Email,
                FaxNumber = "",
                FirstName = "",
                LastName = "",
                PhoneNumber = "",
                Salutation = hepsiburadaShipmentPackage.CompanyName,
                TaxOffice = hepsiburadaShipmentPackage.TaxOffice,
                TaxOfficeNumber = hepsiburadaShipmentPackage.TaxNumber + hepsiburadaShipmentPackage.IdentityNo,
                Town = "",
                Title = hepsiburadaShipmentPackage.CompanyName,
                ZipPostalCode = hepsiburadaShipmentPackage.BillingPostalCode,
            };
            order.ShippingAddress = new DTOs.Address.AddressDto()
            {
                Address1 = hepsiburadaShipmentPackage.ShippingAddressDetail,
                Address2 = "",
                AddressType = "Fatura Adresi",
                City = "",
                Company = hepsiburadaShipmentPackage.CompanyName,
                Country = "",
                District = "",
                Email = hepsiburadaShipmentPackage.Email,
                FaxNumber = "",
                FirstName = "",
                LastName = "",
                PhoneNumber = "",
                Salutation = hepsiburadaShipmentPackage.CompanyName,
                TaxOffice = hepsiburadaShipmentPackage.TaxOffice,
                TaxOfficeNumber = hepsiburadaShipmentPackage.TaxNumber + hepsiburadaShipmentPackage.IdentityNo,
                Town = "",
                Title = hepsiburadaShipmentPackage.CompanyName,
                ZipPostalCode = "",
            };
            order.OrderItems = hepsiburadaShipmentPackage.Items.Select(m => new DTOs.OrderItem.OrderItemDto()
            {
                Product = new DTOs.Product.ProductDto()
                {
                    Code = m.MerchantSku,
                },
                Quantity = m.Quantity,
                Price = m.MerchantTotalPrice.Amount,
                UnitPrice = m.MerchantUnitPrice.Amount,
                TaxRate = m.Vat,
                DiscountAmount = m.TotalMerchantDiscount.Amount,
                IntegrationSku = m.MerchantSku,
                IntegrationProductName = m.ProductName,
                ItemWeight = 0,
                AttributesXml = "",
                ProductCost = 0,
                Sku = m.MerchantSku,
            }).ToList();

            ShipmentDto shipmentDto = new ShipmentDto();
            shipmentDto.OrderId = 0;
            shipmentDto.Carrier = hepsiburadaShipmentPackage.CargoCompany;
            shipmentDto.PackageNo = hepsiburadaShipmentPackage.PackageNumber;
            shipmentDto.TrackingNumber = hepsiburadaShipmentPackage.Barcode;
            shipmentDto.TrackingUrl = "";
            shipmentDto.TotalWeight = 0;
            shipmentDto.ShippedDate = null;
            shipmentDto.DeliveryDate = null;
            shipmentDto.CreatedOn = DateTime.UtcNow;

            order.Shipments.Add(shipmentDto);





            return order;
        }

        public static IEnumerable<OrderDto> ToDtoList(IEnumerable<HepsiburadaShipmentPackageDto> hepsiburadaShipmentPackages)
        {
            if (hepsiburadaShipmentPackages == null)
                yield break;

            foreach (var hepsiburadaShipmentPackage in hepsiburadaShipmentPackages)
            {
                var dto = ToDto(hepsiburadaShipmentPackage);
                if (dto != null)
                    yield return dto;
            }
        }
    }
}
