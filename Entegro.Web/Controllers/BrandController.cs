using Entegro.Application.Interfaces.Services;
using Entegro.Application.Services;
using Entegro.Web.Models;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public async Task<IActionResult> BrandList([FromBody] DatatableData model)
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
