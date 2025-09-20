using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductAttribute;
using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models.Catalog.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    [Authorize]
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

        public async Task<IActionResult> CreateOrUpdate(int id)
        {
            ProductAttributeModel model = new ProductAttributeModel();
            if (id > 0)
            {
                var productAttribute = await _productAttributeService.GetByIdAsync(id);
                if (productAttribute != null)
                {
                    model.Id = productAttribute.Id;
                    model.DisplayOrder = productAttribute.DisplayOrder;
                    model.Description = productAttribute.Description;
                    model.Name = productAttribute.Name;
                }
            }
            return PartialView("_CreateOrUpdate", model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(ProductAttributeModel model)
        {
            if (model.Id > 0)
            {
                var updateModelDto = new UpdateProductAttributeDto
                {
                    Description = model.Description,
                    Name = model.Name,
                    Id = model.Id,
                    DisplayOrder = model.DisplayOrder,
                };
                await _productAttributeService.UpdateAsync(updateModelDto);
                return Json(new { success = true });
            }

            var createModelDto = new CreateProductAttributeDto
            {
                Description = model.Description,
                Name = model.Name,
                DisplayOrder = model.DisplayOrder
            };
            await _productAttributeService.AddAsync(createModelDto);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _productAttributeService.DeleteAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProductAttributeList([FromBody] GridCommand gridCommand)
        {
            var result = await _productAttributeService.GetPagedAsync(gridCommand);

            return Json(new
            {
                draw = gridCommand.Draw,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.TotalCount,
                data = result.Items
            });

        }
    }
}
