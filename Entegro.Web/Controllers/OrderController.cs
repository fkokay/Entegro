using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models;
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
                Id = id,
                CustomerId = orderDetail.CustomerId,
                Deleted = orderDetail.Deleted,
                IsTransient = orderDetail.IsTransient,
                OrderNo = orderDetail.OrderNo,
                OrderSource = orderDetail.OrderSource,
                TotalAmount = orderDetail.TotalAmount,
                OrderSourceId = orderDetail.OrderSourceId,
                OrderDate = orderDetail.OrderDate,
                CalculateTotalAmount = orderDetail.CalculateTotalAmount(),
                Customer = new CustomerViewModel
                {
                    Id = orderDetail.CustomerId,
                    Name = orderDetail.Customer.Name,
                    Address = orderDetail.Customer.Address,
                    Email = orderDetail.Customer.Email,
                },
                OrderItems = orderDetail.OrderItems.Select(i => new OrderItemViewModel
                {
                    Id = i.Id,
                    DiscountAmount = i.DiscountAmount,
                    UnitPrice = i.UnitPrice,
                    TaxRate = i.TaxRate,
                    OrderId = i.OrderId,
                    Price = i.Price,
                    Quantity = i.Quantity,
                    ProductId = i.ProductId,
                    Product = new ProductViewModel
                    {
                        Id = i.ProductId,
                        Name = i.Product.Name,
                        PictureUrl = i.Product.MainPicture?.Url
                    },
                }).ToList()
            };
            return View(orderDetailViewModel);
        }
    }
}
