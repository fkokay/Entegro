using Entegro.Application.DTOs.ProductAttributeValue;
using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class ProductAttributeValueController : Controller
    {
        private readonly IProductAttributeValueService _productAttributeValueService;
        private readonly IProductAttributeService _productAttributeService;
        public ProductAttributeValueController(IProductAttributeValueService productAttributeValueService, IProductAttributeService productAttributeService)
        {
            _productAttributeValueService = productAttributeValueService ?? throw new ArgumentNullException(nameof(productAttributeValueService));
            _productAttributeService = productAttributeService;
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
        public async Task<IActionResult> Create(CreateProductAttributeValueViewModel model)
        {
            var createDto = new CreateProductAttributeValueDto
            {
                Name = model.Name,
                DisplayOrder = model.DisplayOrder,
                ProductAttributeId = model.ProductAttributeId
            };
            await _productAttributeValueService.AddAsync(createDto);

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var productAttributeValue = await _productAttributeValueService.GetByIdAsync(id);
            if (productAttributeValue == null)
            {
                return NotFound();
            }

            var productAttributeValueModel = new ProductAttributeValueViewModel
            {
                ProductAttributeId = productAttributeValue.ProductAttributeId,
                DisplayOrder = productAttributeValue.DisplayOrder,
                Name = productAttributeValue.Name,
                Id = productAttributeValue.Id
            };


            return Json(productAttributeValueModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateProductAttributeValueViewModel model)
        {
            if (ModelState.IsValid)
            {
                var updateDto = new UpdateProductAttributeValueDto
                {
                    Id = model.Id,
                    Name = model.Name,
                    ProductAttributeId = model.ProductAttributeId,
                    DisplayOrder = model.DisplayOrder
                };
                await _productAttributeValueService.UpdateAsync(updateDto);
                return Json(new { success = true });
            }
            return Json(new { success = false });
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
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var isSuccess = await _productAttributeValueService.DeleteAsync(id);
            if (isSuccess)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Silinecek Varyant Değeri Bulunamadı" });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProductAttribute()
        {
            var data = await _productAttributeService.GetAllAsync();
            var results = data.Select(d => new { id = d.Id, text = d.Name, });
            return Json(new { results });
        }
    }
}
