using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models;
using Entegro.Web.Models.Catalog.Products;
using Entegro.Web.Models.Checkout.Orders;
using Entegro.Web.Models.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        public IActionResult Index()
        {
            return List();
        }

        public IActionResult List()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> OrderList([FromBody] GridCommand model)
        {
            int pageNumber = model.Start / model.Length;
            int pageSize = model.Length;


            var result = await _orderService.GetPagedAsync(pageNumber, model.Length);

            return Json(new
            {
                draw = model.Draw,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.TotalCount,
                data = result.Items
            });
        }
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var orderDetail = await _orderService.GetOrderByIdAsync(id);
            var orderDetailViewModel = new OrderViewModel
            {
                Id = orderDetail.Id,
                OrderSourceId = orderDetail.OrderSourceId,
                OrderNumber = orderDetail.OrderNumber,
                OrderGuid = orderDetail.OrderGuid,
                CustomerId = orderDetail.CustomerId,
                Customer = new CustomerViewModel
                {
                    Id = orderDetail.Customer.Id,
                    UpdatedOn = orderDetail.Customer.UpdatedOnUtc,
                    Address = orderDetail.Customer.Address,
                    City = orderDetail.Customer.City,
                    CreatedOn = orderDetail.Customer.CreatedOnUtc,
                    District = orderDetail.Customer.District,
                    CustomerType = orderDetail.Customer.CustomerType,
                    Email = orderDetail.Customer.Email,
                    Name = orderDetail.Customer.Name,
                    PhoneNumber = orderDetail.Customer.PhoneNumber,
                    Street = orderDetail.Customer.Street,
                    Town = orderDetail.Customer.Town,
                    TaxOffice = orderDetail.Customer.TaxOffice,
                    TaxNumber = orderDetail.Customer.TaxNumber,
                },
                BillingAddressId = orderDetail.BillingAddressId == null ? null : orderDetail.BillingAddressId,
                BillingAddress = orderDetail.BillingAddress == null ? null : new AddressViewModel
                {
                    CityId = orderDetail.BillingAddress.CityId,
                    LastName = orderDetail.BillingAddress.LastName,
                    Address1 = orderDetail.BillingAddress.Address1,
                    Address2 = orderDetail.BillingAddress.Address2,
                    AddressType = orderDetail.BillingAddress.AddressType,
                    Company = orderDetail.BillingAddress.Company,
                    CountryId = orderDetail.BillingAddress.CountryId,
                    CreatedOn = orderDetail.BillingAddress.CreatedOn,
                    DistrictId = orderDetail.BillingAddress.DistrictId,
                    Email = orderDetail.BillingAddress.Email,
                    FaxNumber = orderDetail.BillingAddress.FaxNumber,
                    FirstName = orderDetail.BillingAddress.FirstName,
                    Id = orderDetail.BillingAddress.Id,
                    PhoneNumber = orderDetail.BillingAddress.PhoneNumber,
                    Salutation = orderDetail.BillingAddress.Salutation,
                    TaxOffice = orderDetail.BillingAddress.TaxOffice,
                    TaxOfficeNumber = orderDetail.BillingAddress.TaxOfficeNumber,
                    Title = orderDetail.BillingAddress.Title,
                    TownId = orderDetail.BillingAddress.Id,
                    UpdatedOn = orderDetail.BillingAddress.UpdatedOn,
                    ZipPostalCode = orderDetail.BillingAddress.ZipPostalCode,
                },
                ShippingAddressId = orderDetail.ShippingAddressId == null ? null : orderDetail.ShippingAddressId,
                ShippingAddress = orderDetail.ShippingAddress == null ? null : new AddressViewModel
                {
                    CityId = orderDetail.ShippingAddress.CityId,
                    LastName = orderDetail.ShippingAddress.LastName,
                    Address1 = orderDetail.ShippingAddress.Address1,
                    Address2 = orderDetail.ShippingAddress.Address2,
                    AddressType = orderDetail.ShippingAddress.AddressType,
                    Company = orderDetail.ShippingAddress.Company,
                    CountryId = orderDetail.ShippingAddress.CountryId,
                    CreatedOn = orderDetail.ShippingAddress.CreatedOn,
                    DistrictId = orderDetail.ShippingAddress.DistrictId,
                    Email = orderDetail.ShippingAddress.Email,
                    FaxNumber = orderDetail.ShippingAddress.FaxNumber,
                    FirstName = orderDetail.ShippingAddress.FirstName,
                    Id = orderDetail.ShippingAddress.Id,
                    PhoneNumber = orderDetail.ShippingAddress.PhoneNumber,
                    Salutation = orderDetail.ShippingAddress.Salutation,
                    TaxOffice = orderDetail.ShippingAddress.TaxOffice,
                    TaxOfficeNumber = orderDetail.ShippingAddress.TaxOfficeNumber,
                    Title = orderDetail.ShippingAddress.Title,
                    TownId = orderDetail.ShippingAddress.Id,
                    UpdatedOn = orderDetail.ShippingAddress.UpdatedOn,
                    ZipPostalCode = orderDetail.ShippingAddress.ZipPostalCode,
                },
                PaymentMethodSystemName = orderDetail.PaymentMethodSystemName,
                OrderDate = orderDetail.OrderDate,
                CurrencyRate = orderDetail.CurrencyRate,
                VatNumber = orderDetail.VatNumber,
                OrderSubtotalInclTax = orderDetail.OrderSubtotalInclTax,
                OrderSubtotalExclTax = orderDetail.OrderSubtotalExclTax,
                OrderSubTotalDiscountInclTax = orderDetail.OrderSubTotalDiscountInclTax,
                OrderSubTotalDiscountExclTax = orderDetail.OrderSubTotalDiscountExclTax,
                OrderShippingInclTax = orderDetail.OrderShippingInclTax,
                OrderShippingExclTax = orderDetail.OrderShippingExclTax,
                OrderShippingTaxRate = orderDetail.OrderShippingTaxRate,
                PaymentMethodAdditionalFeeExclTax = orderDetail.PaymentMethodAdditionalFeeExclTax,
                PaymentMethodAdditionalFeeInclTax = orderDetail.PaymentMethodAdditionalFeeInclTax,
                PaymentMethodAdditionalFeeTaxRate = orderDetail.PaymentMethodAdditionalFeeTaxRate,
                OrderTax = orderDetail.OrderTax,
                OrderDiscount = orderDetail.OrderDiscount,
                OrderTotal = orderDetail.OrderTotal,
                RefundedAmount = orderDetail.RefundedAmount,
                CustomerIp = orderDetail.CustomerIp,
                Deleted = orderDetail.Deleted,
                IsTransient = orderDetail.IsTransient,
                TaxRates = orderDetail.TaxRates,
                PaidDateUtc = orderDetail.PaidDateUtc,
                ShippingMethod = orderDetail.ShippingMethod,
                OrderStatusId = orderDetail.OrderStatusId,
                OrderStatus = orderDetail.OrderStatus,
                PaymentStatusId = orderDetail.PaymentStatusId,
                PaymentStatus = orderDetail.PaymentStatus,
                ShippingStatusId = orderDetail.ShippingStatusId,
                ShippingStatus = orderDetail.ShippingStatus,
                OrderItems = orderDetail.OrderItems.Select(oi => new OrderItemViewModel
                {
                    OrderId = oi.OrderId,
                    DiscountAmount = oi.DiscountAmount,
                    Id = oi.Id,
                    Price = oi.Price,
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    TaxRate = oi.TaxRate,
                    UnitPrice = oi.UnitPrice,
                    Product = new ProductViewModel
                    {
                        Id = oi.ProductId,
                        PictureUrl = oi.Product.MainPicture?.Url,
                        Name = oi.Product.Name,
                    }
                }).ToList(),
                CalculateTotalAmount = orderDetail.CalculateTotalAmount(),
                OrderNotes = orderDetail.OrderNotes.Select(on => new OrderNoteViewModel
                {
                    Id = on.OrderId,
                    CreatedOnUtc = on.CreatedOnUtc,
                    Note = on.Note,
                    OrderId = on.OrderId,
                }).ToList()
            };
            return View(orderDetailViewModel);
        }
    }
}
