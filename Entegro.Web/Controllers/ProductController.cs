using Entegro.Application.DTOs.Product;
using Entegro.Application.DTOs.ProductCategory;
using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Entegro.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IProductCategoryMappingService _productCategoryMappingService;
        private readonly IBrandService _brandService;
        public ProductController(IProductService productService, IProductCategoryMappingService productCategoryMappingService, IBrandService brandService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _productCategoryMappingService = productCategoryMappingService ?? throw new ArgumentNullException(nameof(productCategoryMappingService));
            _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
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
            ProductViewModel model = new ProductViewModel();
            await PrepareProductModel(model, null);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var createDto = new CreateProductDto();
                createDto.Barcode = model.Barcode;
                createDto.BrandId = model.BrandId;
                createDto.Code = model.Code;
                createDto.Currency = model.Currency;
                createDto.Description = model.Description;
                createDto.Height = model.Height;
                createDto.Length = model.Length;
                createDto.MetaDescription = model.MetaDescription;
                createDto.MetaTitle = model.MetaTitle;
                createDto.MetaKeywords = model.MetaKeywords;
                createDto.Name = model.Name;
                createDto.Price = model.Price;
                createDto.StockQuantity = model.StockQuantity;
                createDto.Unit = model.Unit;
                createDto.VatInc = model.VatInc;
                createDto.VatRate = model.VatRate;
                createDto.Weight = model.Weight;
                createDto.Width = model.Width;

                await _productService.CreateProductAsync(createDto);

                return Json(new { success = true });
            }

            await PrepareProductModel(model, null);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ProductViewModel model = new ProductViewModel();

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await PrepareProductModel(model, product);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var updateDto = new UpdateProductDto();
                updateDto.Barcode = model.Barcode;
                updateDto.BrandId = model.BrandId;
                updateDto.Code = model.Code;
                updateDto.Currency = model.Currency;
                updateDto.Description = model.Description;
                updateDto.Height = model.Height;
                updateDto.Length = model.Length;
                updateDto.MetaDescription = model.MetaDescription;
                updateDto.MetaTitle = model.MetaTitle;
                updateDto.MetaKeywords = model.MetaKeywords;
                updateDto.Name = model.Name;
                updateDto.Price = model.Price;
                updateDto.StockQuantity = model.StockQuantity;
                updateDto.Unit = model.Unit;
                updateDto.VatInc = model.VatInc;
                updateDto.VatRate = model.VatRate;
                updateDto.Weight = model.Weight;
                updateDto.Width = model.Width;

                await _productService.UpdateProductAsync(updateDto);

                return Json(new { success = true });
            }

            await PrepareProductModel(model, null);
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


        private async Task PrepareProductModel(ProductViewModel model, ProductDto? product)
        {
            if (product != null)
            {
                model.Id = product.Id;
                model.Barcode = product.Barcode;
                model.BrandId = product.BrandId;
                model.Code = product.Code;
                model.Currency = product.Currency;
                model.Description = product.Description;
                model.Height = product.Height;
                model.Length = product.Length;
                model.MetaDescription = product.MetaDescription;
                model.MetaTitle = product.MetaTitle;
                model.MetaKeywords = product.MetaKeywords;
                model.Name = product.Name;
                model.VatInc = product.VatInc;
                model.Price = product.Price;
                model.StockQuantity = product.StockQuantity;
                model.Unit = product.Unit;
                model.VatRate = product.VatRate;
                model.Weight = product.Weight;
                model.Width = product.Width;
            }

            var brands = await _brandService.GetBrandsAsync();

            ViewBag.Brands = brands.Select(m => new SelectListItem()
            {
                Text = m.Name,
                Value = m.Id.ToString()
            }).ToList();
            ViewBag.Currencies = new List<SelectListItem>
            {
                new SelectListItem { Text = "TRY", Value = "TRY" },
                new SelectListItem { Text = "USD", Value = "USD" },
                new SelectListItem { Text = "EUR", Value = "EUR" },
                new SelectListItem { Text = "GBP", Value = "GBP" },
                new SelectListItem { Text = "JPY", Value = "JPY" }
            };
            ViewBag.VatRates = new List<SelectListItem>
            {
                new SelectListItem { Text = "0%", Value = "0" },
                new SelectListItem { Text = "1%", Value = "1" },
                new SelectListItem { Text = "8%", Value = "8" },
                new SelectListItem { Text = "18%", Value = "18" },
                new SelectListItem { Text = "20%", Value = "20" }
            };
            ViewBag.Units = new List<SelectListItem>
            {
                new SelectListItem { Text = "Adet", Value = "Adet" },
                new SelectListItem { Text = "Kg", Value = "Kg" },
                new SelectListItem { Text = "Litre", Value = "Litre" },
                new SelectListItem { Text = "Metre", Value = "Metre" },
                new SelectListItem { Text = "Kutu", Value = "Kutu" }
            };
        }

        [HttpGet]
        public async Task<IActionResult> Get(int productId, CancellationToken ct)
        {
            var data = await _productCategoryMappingService.GetCategoryPathsByProductAsync(productId, ct);
            var results = data.Select(d => new { id = d.Id, text = d.CategoryPath, displayOrder = d.DisplayOrder });
            return Json(new { results });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteProductCategory(int id)
        {
            bool isSuccess = await _productCategoryMappingService.DeleteProductCategoryAsync(id);
            return Json(new { success = isSuccess });
        }


        [HttpPost]
        public async Task<IActionResult> CreateProductCategory([FromBody] CreateProductCategoryDto createProductCategoryDto)
        {
            if (ModelState.IsValid)
            {
                int id = await _productCategoryMappingService.CreateProductCategoryAsync(createProductCategoryDto);
                return Json(new { success = true, id });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }
    }
}
