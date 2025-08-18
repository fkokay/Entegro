using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class ProductAttributeValueController : Controller
    {
        private readonly IProductAttributeValueService _productAttributeValueService;
        public ProductAttributeValueController(IProductAttributeValueService productAttributeValueService)
        {
            _productAttributeValueService = productAttributeValueService ?? throw new ArgumentNullException(nameof(productAttributeValueService));
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
        public async Task<IActionResult> ProductAttributeValueList([FromBody] GridCommand model)
        {

            int pageNumber = model.Start / model.Length;
            int pageSize = model.Length;


            var result = await _productAttributeValueService.GetAllAsync(pageNumber, model.Length);

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
