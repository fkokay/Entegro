using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Product;
using Entegro.Application.Interfaces.Services;
using Entegro.Application.Services;
using Entegro.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Entegro.Web.Controllers
{
    public class BrandController : Controller
    {
        private readonly IBrandService _brandService;
        public BrandController(IBrandService brandService)
        {
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

        public IActionResult Create()
        {
            BrandDto model = new BrandDto();
            model.DisplayOrder = 0;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBrandDto model)
        {
            if (ModelState.IsValid)
            {
                await _brandService.CreateBrandAsync(model);
                return RedirectToAction(nameof(List));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var brand = await _brandService.GetBrandByIdAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateBrandDto model)
        {
            if (ModelState.IsValid)
            {
                await _brandService.UpdateBrandAsync(model);
                return RedirectToAction(nameof(List));
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> BrandList([FromBody] GridCommand model)
        {
            var result = await _brandService.GetBrandsAsync(model.Draw, model.Length);

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
