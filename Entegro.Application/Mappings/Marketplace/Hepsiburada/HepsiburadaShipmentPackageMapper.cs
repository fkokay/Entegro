using Entegro.Application.DTOs.Marketplace.Hepsiburada;
using Entegro.Application.DTOs.Marketplace.N11;
using Entegro.Application.DTOs.Order;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            order.OrderSource = Domain.Enums.OrderSource.Hepsiburada;
            order.OrderNumber = hepsiburadaShipmentPackage.Id;
            order.OrderGuid = Guid.Parse(hepsiburadaShipmentPackage.Id);
            order.OrderTax = 0;
            order.OrderDiscount = 0;
            order.RefundedAmount = 0;
            order.OrderSubtotalInclTax = 0;
            order.OrderSubtotalExclTax = 0;
            order.OrderSubTotalDiscountInclTax = 0;
            order.OrderSubTotalDiscountExclTax = 0;
            order.OrderShippingExclTax = 0;
            order.OrderShippingInclTax = 0;
            order.OrderShippingTaxRate = 0;
            order.PaymentMethodAdditionalFeeExclTax = 0;
            order.PaymentMethodAdditionalFeeInclTax = 0;
            order.PaymentMethodAdditionalFeeTaxRate = 0;
            order.OrderTotal = hepsiburadaShipmentPackage.TotalPrice.Amount;
            order.PaymentMethodSystemName = "Hepsiburada";
            order.OrderDate = hepsiburadaShipmentPackage.OrderDate;
            order.CurrencyRate = 1;
            order.VatNumber = "";
            order.CustomerIp = "127.0.0.1";
            order.Deleted = false;
            order.IsTransient = false;
            order.OrderStatus =  Domain.Enums.OrderStatus.Pending;
            order.PaymentStatus =  Domain.Enums.PaymentStatus.Paid;
            order.ShippingMethod = hepsiburadaShipmentPackage.CargoCompany;
            order.ShippingStatus = Domain.Enums.ShippingStatus.Shipped;
            order.TaxRates = "";
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
                CityId = 0,
                Company = hepsiburadaShipmentPackage.CompanyName,
                CountryId = 0,
                DistrictId = 0,
                Email = hepsiburadaShipmentPackage.Email,
                FaxNumber = "",
                FirstName = "",
                LastName = "",
                PhoneNumber = "",
                Salutation = hepsiburadaShipmentPackage.CompanyName,
                TaxOffice = hepsiburadaShipmentPackage.TaxOffice,
                TaxOfficeNumber = hepsiburadaShipmentPackage.TaxNumber + hepsiburadaShipmentPackage.IdentityNo,
                TownId = 0,
                Title = hepsiburadaShipmentPackage.CompanyName,
                ZipPostalCode = hepsiburadaShipmentPackage.BillingPostalCode,
            };
            order.ShippingAddress = new DTOs.Address.AddressDto()
            {
                Address1 = hepsiburadaShipmentPackage.ShippingAddressDetail,
                Address2 = "",
                AddressType = "Fatura Adresi",
                CityId = 0,
                Company = hepsiburadaShipmentPackage.CompanyName,
                CountryId = 0,
                DistrictId = 0,
                Email = hepsiburadaShipmentPackage.Email,
                FaxNumber = "",
                FirstName = "",
                LastName = "",
                PhoneNumber = "",
                Salutation = hepsiburadaShipmentPackage.CompanyName,
                TaxOffice = hepsiburadaShipmentPackage.TaxOffice,
                TaxOfficeNumber = hepsiburadaShipmentPackage.TaxNumber + hepsiburadaShipmentPackage.IdentityNo,
                TownId = 0,
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
            }).ToList();





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
