using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        private readonly IBrandService _brandService;
        public ProductController(IProductService productService, IBrandService brandService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _brandService = brandService;
        }
        public IActionResult Index()
        {
            return List();
        }

        public IActionResult List()
        {
            return View();
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Brands = await _brandService.GetBrandsAsync();
            ProductViewModel model = new ProductViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProductList([FromBody] GridCommand model)
        {
            int pageNumber = model.Start / model.Length;
            int pageSize = model.Length;


            var result = await _productService.GetProductsAsync(pageNumber, model.Length);

            return Json(new
            {
                draw = model.Draw,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.TotalCount,
                data = result.Items
            });
        }
    }
}
