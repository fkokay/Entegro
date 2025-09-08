using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductAttributeValue;
using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models.Catalog.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Entegro.Web.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> CreateOrUpdate(int id)
        {
            ProductAttributeValueViewModel model = new ProductAttributeValueViewModel();
            var productAttributes = await _productAttributeService.GetAllAsync();
            ViewBag.ProductAttributes = productAttributes.Select(m => new SelectListItem()
            {
                Text = m.Name,
                Value = m.Id.ToString()
            }).ToList();

            if (id > 0)
            {
                var productAttributeValue = await _productAttributeValueService.GetByIdAsync(id);
                if (productAttributeValue != null)
                {
                    model.Id = productAttributeValue.Id;
                    model.ProductAttributeId = productAttributeValue.ProductAttributeId;
                    model.DisplayOrder = productAttributeValue.DisplayOrder;
                    model.Name = productAttributeValue.Name;
                }
            }
            return PartialView("_CreateOrUpdate", model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(ProductAttributeValueViewModel model)
        {
            if (model.Id > 0)
            {
                var updateModelDto = new UpdateProductAttributeValueDto
                {
                    ProductAttributeId = model.ProductAttributeId,
                    Name = model.Name,
                    Id = model.Id,
                    DisplayOrder = model.DisplayOrder,
                };
                await _productAttributeValueService.UpdateAsync(updateModelDto);
                return Json(new { success = true });
            }

            var createModelDto = new CreateProductAttributeValueDto
            {
                ProductAttributeId = model.ProductAttributeId,
                Name = model.Name,
                DisplayOrder = model.DisplayOrder,
            };
            await _productAttributeValueService.AddAsync(createModelDto);
            return Json(new { success = true });
        }
        //[HttpPost]
        //public async Task<IActionResult> Create(ProductAttributeValueViewModel model)
        //{
        //    var createDto = new CreateProductAttributeValueDto
        //    {
        //        Name = model.Name,
        //        DisplayOrder = model.DisplayOrder,
        //        ProductAttributeId = model.ProductAttributeId
        //    };
        //    await _productAttributeValueService.AddAsync(createDto);

        //    return Json(new { success = true });
        //}

        //[HttpGet]
        //public async Task<IActionResult> Edit(int id)
        //{
        //    var productAttributeValue = await _productAttributeValueService.GetByIdAsync(id);
        //    if (productAttributeValue == null)
        //    {
        //        return NotFound();
        //    }

        //    var productAttributeValueModel = new ProductAttributeValueViewModel
        //    {
        //        ProductAttributeId = productAttributeValue.ProductAttributeId,
        //        DisplayOrder = productAttributeValue.DisplayOrder,
        //        Name = productAttributeValue.Name,
        //        Id = productAttributeValue.Id
        //    };


        //    return Json(productAttributeValueModel);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Edit(ProductAttributeValueViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var updateDto = new UpdateProductAttributeValueDto
        //        {
        //            Id = model.Id,
        //            Name = model.Name,
        //            ProductAttributeId = model.ProductAttributeId,
        //            DisplayOrder = model.DisplayOrder
        //        };
        //        await _productAttributeValueService.UpdateAsync(updateDto);
        //        return Json(new { success = true });
        //    }
        //    return Json(new { success = false });
        //}
        [HttpPost]
        public async Task<IActionResult> ProductAttributeValueList([FromBody] GridCommand gridCommand)
        {
            var result = await _productAttributeValueService.GetPagedAsync(gridCommand);

            return Json(new
            {
                draw = gridCommand.Draw,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.TotalCount,
                data = result.Items
            });

        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _productAttributeValueService.DeleteAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
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
