using Entegro.Application.DTOs.Product;
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

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Brands = await _brandService.GetBrandsAsync();
            ProductViewModel model = new ProductViewModel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel model, List<int> CategoryIds)
        {

            var createDto = new CreateProductDto
            {
                Barcode = model.Barcode,
                BrandId = model.BrandId,
                Code = model.Code,
                Currency = model.Currency,
                Description = model.Description,
                Height = model.Height,
                Length = model.Length,
                MetaDescription = model.MetaDescription,
                MetaTitle = model.MetaTitle,
                MetaKeywords = model.MetaKeywords,
                Name = model.Name,
                Price = model.Price,
                StockQuantity = model.StockQuantity,
                Unit = model.Unit,
                VatInc = model.VatInc,
                VatRate = model.VatRate,
                Weight = model.Weight,
                Width = model.Width,

            };

            await _productService.CreateProductAsync(createDto);

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Brands = await _brandService.GetBrandsAsync();
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var productModel = new ProductViewModel
            {
                Id = product.Id,
                Barcode = product.Barcode,
                BrandId = product.BrandId,
                Code = product.Code,
                Currency = product.Currency,
                Description = product.Description,
                Height = product.Height,
                Length = product.Length,
                MetaDescription = product.MetaDescription,
                MetaTitle = product.MetaTitle,
                MetaKeywords = product.MetaKeywords,
                Name = product.Name,
                VatInc = product.VatInc,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                Unit = product.Unit,
                VatRate = product.VatRate,
                Weight = product.Weight,
                Width = product.Width,
            };
            return View(productModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var updateDto = new UpdateProductDto
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description,
                    MetaDescription = model.MetaDescription,
                    MetaTitle = model.MetaTitle,
                    MetaKeywords = model.MetaKeywords,
                    Price = model.Price,
                    Currency = model.Currency,
                    Unit = model.Unit,
                    VatRate = model.VatRate,
                    VatInc = model.VatInc,
                    Barcode = model.Barcode,
                    BrandId = model.BrandId,
                    Code = model.Code,
                    StockQuantity = model.StockQuantity,
                    Height = model.Height,
                    Length = model.Length,
                    Width = model.Width,
                    Weight = model.Weight,
                };
                await _productService.UpdateProductAsync(updateDto);
                return Json(new { success = true });
            }
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
