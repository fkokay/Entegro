using Entegro.Application.Interfaces.Services;
using Entegro.Infrastructure.Data;
using Entegro.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Entegro.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
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
        public async Task<IActionResult> ProductList([FromBody] GridCommand model)
        {
            var result = await _productService.GetProductsAsync(model.Draw, model.Length);

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
