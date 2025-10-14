using Entegro.Application.Interfaces.Services.Base;
using Entegro.Web.Models.Catalog.Products;
using Entegro.Web.Models.Checkout.Orders;
using Entegro.Web.Models.Dashboard;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Components
{
    public class DashboardViewComponent : ViewComponent
    {
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IOrderItemService _orderItemService;

        public DashboardViewComponent(ICustomerService customerService, IProductService productService, IOrderService orderService, IMapper mapper, IOrderItemService orderItemService)
        {
            _customerService = customerService;
            _productService = productService;
            _orderService = orderService;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _orderItemService = orderItemService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string dashboardType)
        {

            if (dashboardType == "Statistics")
            {
                StatisticsViewModel model = new StatisticsViewModel();
                model.TotalOrderPrice = await _orderService.GetTotalSalesAsync();
                model.CompleteOrderStatusCount = await _orderService.CompleteOrderStatusCount();
                model.CustomerCount = await _customerService.GetCustomerCount();
                model.ProductCount = await _productService.GetProductCountAsync();
                return View(dashboardType, model);
            }
            if (dashboardType == "ViewSales")
            {
                ViewSalesViewModel model = new ViewSalesViewModel();
                model.TotalOrderPrice = await _orderService.GetTotalSalesAsync();
                return View(dashboardType, model);
            }

            else if (dashboardType == "GeneratedLeads")
            {
                GeneratedLeadsViewModel model = new GeneratedLeadsViewModel();
                model.CurrentMonthCustomerCount = await _customerService.GetCurrentMonthCustomerCountAsync();
                return View(dashboardType, model);
            }

            else if (dashboardType == "PopularProduct")
            {
                var productsDto = await _productService.GetTopOrRandomProductsAsync();
                var productModels = new PopularProductViewModel()
                {
                    Products = _mapper.Map<List<ProductModel>>(productsDto)
                };
                return View(dashboardType, productModels);
            }

            else if (dashboardType == "GetLast10OrdersWithItems")
            {
                var orderDtos = await _orderService.GetLast10OrdersWithItemsAsync();
                var order = _mapper.Map<List<OrderModel>>(orderDtos);
                return View(dashboardType, order);
            }

            else if (dashboardType == "InvoiceTable")
            {
                return View(dashboardType);
            }

            return View(dashboardType);
        }
    }
}
