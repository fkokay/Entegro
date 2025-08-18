using Entegro.Application.DTOs.ProductAttribute;
using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class ProductAttributeController : Controller
    {
        private readonly IProductAttributeService _productAttributeService;
        public ProductAttributeController(IProductAttributeService productAttributeService)
        {
            _productAttributeService = productAttributeService ?? throw new ArgumentNullException(nameof(productAttributeService));
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductAttributeViewModel model)
        {
            var createDto = new CreateProductAttributeDto
            {
                Name = model.Name,
                Description = model.Description,
                DisplayOrder = model.DisplayOrder,
            };
            await _productAttributeService.AddAsync(createDto);

            return Json(new { success = true });
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var productAttribute = await _productAttributeService.GetByIdAsync(id);
            if (productAttribute == null)
            {
                return NotFound();
            }

            var productAttributeModel = new ProductAttributeViewModel
            {
                Description = productAttribute.Description,
                DisplayOrder = productAttribute.DisplayOrder,
                Name = productAttribute.Name,
                Id = productAttribute.Id
            };


            return Json(productAttributeModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateProductAttributeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var updateDto = new UpdateProductAttributeDto
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description,
                    DisplayOrder = model.DisplayOrder
                };
                await _productAttributeService.UpdateAsync(updateDto);
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var isSuccess = await _productAttributeService.DeleteAsync(id);
            if (isSuccess)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Silinecek Varyant Bulunamadı" });
        }

        [HttpPost]
        public async Task<IActionResult> ProductAttributeList([FromBody] GridCommand model)
        {

            int pageNumber = model.Start / model.Length;
            int pageSize = model.Length;


            var result = await _productAttributeService.GetAllAsync(pageNumber, model.Length);

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
