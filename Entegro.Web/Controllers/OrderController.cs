using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.OrderItem;
using Entegro.Application.DTOs.ProductIntegration;
using Entegro.Application.DTOs.Shipment;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities.Checkout;
using Entegro.Web.Models;
using Entegro.Web.Models.Catalog.Products;
using Entegro.Web.Models.Checkout.Orders;
using Entegro.Web.Models.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using System.Threading.Tasks;

namespace Entegro.Web.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IShipmentService _shipmentService;
        private readonly IShipmentItemService _shipmentItemService;
        private readonly IProductIntegrationService _productIntegrationService;
        private readonly IIntegrationSystemService _integrationSystemService;
        private readonly IOrderItemService _orderItemService;
        private readonly IProductService _productService;
        public OrderController(
            IOrderService orderService,
            IShipmentService shipmentService,
            IShipmentItemService shipmentItemService,
            IProductIntegrationService productIntegrationService,
            IIntegrationSystemService integrationSystemService,
            IOrderItemService orderItemService,
            IProductService productService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _shipmentService = shipmentService;
            _shipmentItemService = shipmentItemService;
            _productIntegrationService = productIntegrationService;
            _integrationSystemService = integrationSystemService;
            _orderItemService = orderItemService;
            _productService = productService;
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
        public async Task<IActionResult> OrderList([FromBody] GridCommand gridCommand, int orderStatus)
        {
            var result = await _orderService.GetPagedAsync(gridCommand, orderStatus);
            return Json(new
            {
                draw = gridCommand.Draw,
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
                IntegrationSystemId = orderDetail.IntegrationSystemId,
                OrderNumber = orderDetail.OrderNumber,
                OrderGuid = orderDetail.OrderGuid,
                CustomerId = orderDetail.CustomerId,
                Customer = new CustomerViewModel
                {
                    Id = orderDetail.Customer.Id,
                    UpdatedOn = orderDetail.Customer.UpdatedOn,
                    Address = orderDetail.Customer.Address,
                    City = orderDetail.Customer.City,
                    CreatedOn = orderDetail.Customer.CreatedOn,
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
                OrderTax = orderDetail.OrderTax,
                OrderDiscount = orderDetail.OrderDiscount,
                OrderTotal = orderDetail.OrderTotal,
                RefundedAmount = orderDetail.RefundedAmount,
                Deleted = orderDetail.Deleted,
                IsTransient = orderDetail.IsTransient,
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
                    Product = oi.Product == null ? null : new ProductViewModel
                    {
                        Id = oi.Product.Id,
                        PictureUrl = oi.Product.MainPicture?.Url,
                        Name = oi.Product.Name,
                    }
                }).ToList(),
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

        [HttpPost]
        public async Task<IActionResult> Packaging(int id)
        {
            var orderDetail = await _orderService.GetOrderByIdAsync(id);
            var model = new OrderViewModel
            {
                Id = orderDetail.Id,
                IntegrationSystemId = orderDetail.IntegrationSystemId,
                OrderNumber = orderDetail.OrderNumber,
                OrderGuid = orderDetail.OrderGuid,
                CustomerId = orderDetail.CustomerId,
                Customer = orderDetail.Customer == null ? null : new CustomerViewModel
                {
                    Id = orderDetail.Customer.Id,
                    UpdatedOn = orderDetail.Customer.UpdatedOn,
                    Address = orderDetail.Customer.Address,
                    City = orderDetail.Customer.City,
                    CreatedOn = orderDetail.Customer.CreatedOn,
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
                PaymentMethod = orderDetail.PaymentMethod,
                OrderDateUtc = orderDetail.OrderDateUtc,
                OrderTax = orderDetail.OrderTax,
                OrderDiscount = orderDetail.OrderDiscount,
                OrderTotal = orderDetail.OrderTotal,
                RefundedAmount = orderDetail.RefundedAmount,
                Deleted = orderDetail.Deleted,
                IsTransient = orderDetail.IsTransient,
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
                    Product = oi.Product == null ? null : new ProductViewModel
                    {
                        Id = oi.Product.Id,
                        PictureUrl = oi.Product.MainPicture?.Url,
                        Name = oi.Product.Name,
                        Code = oi.Product.Code,
                        Price = oi.Product.Price
                    }
                }).ToList(),
                OrderNotes = orderDetail.OrderNotes.Select(on => new OrderNoteViewModel
                {
                    Id = on.OrderId,
                    CreatedOnUtc = on.CreatedOnUtc,
                    Note = on.Note,
                    OrderId = on.OrderId,
                }).ToList()
            };
            return PartialView("_Packaging", model);
        }

        [HttpPost]
        public async Task<IActionResult> PackagingSave(string carrier, List<OrderPackageViewModel> orderPackages)
        {
            CreateShipmentDto createShipment = new CreateShipmentDto();
            createShipment.Carrier = carrier;
            createShipment.TrackingNumber = "";
            createShipment.PackageNo = new string(Guid.NewGuid().ToString("N").Where(char.IsDigit).Take(10).ToArray());
            createShipment.TrackingUrl = "";
            createShipment.TotalWeight = 0;
            createShipment.DeliveryDateUtc = null;
            createShipment.ShippedDateUtc = null;
            createShipment.OrderId = orderPackages[0].OrderId;
            createShipment.CreatedOnUtc = DateTime.UtcNow;
            createShipment.ShipmentItems = orderPackages.Where(m => m.IsPackage).Select(m => new Application.DTOs.ShipmentItem.CreateShipmentItemDto()
            {
                OrderItemId = m.OrderItemId,
                Quantity = m.Quantity,
            }).ToList();

            var result = await _shipmentService.AddAsync(createShipment);

            return Json(new { success = true });
        }

        public async Task<IActionResult> Print(int id, string packageNo)
        {
            var orderPrintModel = await _orderService.GetOrderPrintByIdAsync(id, packageNo);

            //return View(orderPrintModel);

            var pdf = new ViewAsPdf("Print", orderPrintModel)
            {
                FileName = "Test.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                CustomSwitches = "--enable-local-file-access"
            };
            var pdfBytes = await pdf.BuildFile(ControllerContext);

            return File(pdfBytes, "application/pdf");
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductIntegration(int integrationSystemId, int productId, int? productVariantAttributeCombinationId, string integrationCode)
        {
            var integrationSystem = await _integrationSystemService.GetByIdAsync(integrationSystemId);
            var product = await _productService.GetProductByIdAsync(productId);

            CreateProductIntegrationDto createProductIntegration = new CreateProductIntegrationDto();
            createProductIntegration.ProductId = productId;
            createProductIntegration.ProductVariantAttributeCombinationId = productVariantAttributeCombinationId;
            createProductIntegration.IntegrationCode = integrationCode;
            createProductIntegration.Price = 0;
            createProductIntegration.IntegrationSystemId = integrationSystemId;
            createProductIntegration.IsSync = true;
            createProductIntegration.Active = true;
            createProductIntegration.LastSyncDate = null;

            await _productIntegrationService.CreateProductIntegrationAsync(createProductIntegration);

            var orderItems = await _orderItemService.GetAllWithIntegrationSkuAsync(integrationCode);
            foreach (var orderItem in orderItems)
            {
                UpdateOrderItemDto updateOrderItem = new UpdateOrderItemDto();
                updateOrderItem.Id = orderItem.Id;
                updateOrderItem.Sku = product.Code;
                updateOrderItem.ProductId = productId;
                updateOrderItem.ProductCost = orderItem.ProductCost;
                updateOrderItem.AttributesXml = orderItem.AttributesXml;
                updateOrderItem.DiscountAmount = orderItem.DiscountAmount;
                updateOrderItem.Quantity = orderItem.Quantity;
                updateOrderItem.Price = orderItem.Price;
                updateOrderItem.UnitPrice = orderItem.UnitPrice;
                updateOrderItem.IntegrationSku = orderItem.IntegrationSku;
                updateOrderItem.IntegrationProductName = orderItem.IntegrationProductName;
                updateOrderItem.ItemWeight = orderItem.ItemWeight;
                updateOrderItem.OrderId = orderItem.OrderId;

                await _orderItemService.UpdateAsync(updateOrderItem);
            }
           

            return Json(new { success = true });
        }
    }
}
