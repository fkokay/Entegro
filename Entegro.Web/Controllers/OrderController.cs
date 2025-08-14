using Entegro.Application.Interfaces.Services;
using Entegro.Application.Services;
using Entegro.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
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
            var result = await _orderService.GetOrdersAsync(model.Draw, model.Length);

            return Json(new
            {
                draw = result.PageNumber,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.TotalCount,
                data = result.Items
            });
        }
    }
}
