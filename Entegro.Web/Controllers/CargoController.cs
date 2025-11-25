using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Order;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Application.Interfaces.Services.Cargo;
using Entegro.Domain.Enums;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class CargoController : Controller
    {
        private readonly IShipmentService _shipmentService;
        private readonly IShipmentItemService _shipmentItemService;
        private readonly IProductIntegrationService _productIntegrationService;
        private readonly IIntegrationSystemService _integrationSystemService;
        private readonly IArasCargoService _arasCargoService;
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        public CargoController(IShipmentItemService shipmentItemService, IProductIntegrationService productIntegrationService, IIntegrationSystemService integrationSystemService, IMapper mapper, IShipmentService shipmentService, IArasCargoService arasCargoService, IOrderService orderService)
        {
            _shipmentItemService = shipmentItemService;
            _productIntegrationService = productIntegrationService;
            _integrationSystemService = integrationSystemService;
            _mapper = mapper;
            _shipmentService = shipmentService;
            _arasCargoService = arasCargoService;
            _orderService = orderService;
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
        public async Task<IActionResult> CancelCargo(string integrationCode, int shippingIntegrationId, int shipmentId)
        {
            var result = await _arasCargoService.CancelDispatch(integrationCode, shippingIntegrationId);
            if (result.ResultCode == "0")
            {
                var shipment = await _shipmentService.GetByIdAsync(shipmentId);
                var order = await _orderService.GetOrderByIdAsync(shipment.OrderId);
                var mapped = _mapper.Map<UpdateOrderDto>(order);
                mapped.OrderStatusId = (int)OrderStatus.Pending;
                await _orderService.UpdateAsync(mapped);
                await _shipmentService.DeleteAsync(shipmentId);

                return Json(new { success = true, message = "Kargonuz Başarıyla İptal Edildi." });
            }

            return BadRequest(new { success = false, message = result.ResultMessage });
        }
        [HttpPost]
        public async Task<IActionResult> GetShipments([FromBody] GridCommand gridCommand)
        {
            var result = await _shipmentService.GetShipmentsByIntegrationIdAsync(gridCommand);

            return Json(new
            {
                draw = gridCommand.Draw,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.TotalCount,
                data = result.Items
            });
        }
    }
}
