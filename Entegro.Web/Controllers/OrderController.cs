using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.OrderItem;
using Entegro.Application.DTOs.ProductIntegration;
using Entegro.Application.DTOs.Shipment;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Web.Models.Checkout.Orders;
using Entegro.Web.Models.Dashboard;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;

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
        private readonly IMapper _mapper;
        public OrderController(
            IOrderService orderService,
            IShipmentService shipmentService,
            IShipmentItemService shipmentItemService,
            IProductIntegrationService productIntegrationService,
            IIntegrationSystemService integrationSystemService,
            IOrderItemService orderItemService,
            IProductService productService,
            IMapper mapper)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _shipmentService = shipmentService;
            _shipmentItemService = shipmentItemService;
            _productIntegrationService = productIntegrationService;
            _integrationSystemService = integrationSystemService;
            _orderItemService = orderItemService;
            _productService = productService;
            _mapper = mapper;
        }

        public Task<IActionResult> Index()
        {
            return List();
        }

        public async Task<IActionResult> List(int orderStatusId = 1)
        {

            ViewBag.OrderStatusId = orderStatusId;
            var orderPage = await _orderService.GetOrderPageAsync();
            var model = _mapper.Map<OrderListModel>(orderPage);

            return View(model);
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
            var order = await _orderService.GetOrderByIdAsync(id);

            var model = _mapper.Map<OrderModel>(order);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Packaging(int id)
        {
            var orderDetail = await _orderService.GetOrderByIdAsync(id);
            var model = _mapper.Map<OrderModel>(orderDetail);
            return PartialView("_Packaging", model);
        }

        [HttpPost]
        public async Task<IActionResult> PackagingSave(string carrier, List<OrderPackageModel> orderPackages)
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
            createShipment.CreatedOn = DateTime.UtcNow;
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

            await _productIntegrationService.AddAsync(createProductIntegration);

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


        [HttpGet]
        public async Task<IActionResult> GetMonthlySales(int year)
        {
            var data = await _orderService.GetMonthlySalesByYearAsync(year);
            return Json(data.Select(d => new { Month = d.Month, TotalAmount = d.TotalAmount }));
        }

        [HttpGet]
        public async Task<IActionResult> GetProductSalesByMarketplace(int groupByType)
        {
            var productSalesDto = await _orderItemService.GetProductSalesByMarketplaceAsync(groupByType);
            var productSales = _mapper.Map<List<ProductSalesViewModel>>(productSalesDto);
            return Json(productSales);
        }
    }
}
