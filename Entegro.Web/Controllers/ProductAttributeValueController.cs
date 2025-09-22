using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductAttribute;
using Entegro.Application.DTOs.ProductAttributeValue;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities.Catalog;
using Entegro.Web.Models.Catalog.Attributes;
using MapsterMapper;
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
        private readonly IMapper _mapper;
        public ProductAttributeValueController(
            IProductAttributeValueService productAttributeValueService,
            IProductAttributeService productAttributeService,
            IMapper mapper)
        {
            _productAttributeValueService = productAttributeValueService ?? throw new ArgumentNullException(nameof(productAttributeValueService));
            _productAttributeService = productAttributeService;
            _mapper = mapper;
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
            if (id > 0)
            {
                var productAttributeValue = await _productAttributeValueService.GetByIdAsync(id);
                if (productAttributeValue == null)
                {
                    return NotFound();
                }

                var model = _mapper.Map<ProductAttributeValueModel>(productAttributeValue);
                await PrepareProductAttributeValueModel(model, productAttributeValue);

                return PartialView("_CreateOrUpdate", model);
            }
            else
            {
                ProductAttributeValueModel model = new ProductAttributeValueModel();
                await PrepareProductAttributeValueModel(model, null);

                return PartialView("_CreateOrUpdate", model);
            }
  
        }

        private async Task PrepareProductAttributeValueModel(ProductAttributeValueModel model, ProductAttributeValueDto? value)
        {
            var productAttributes = await _productAttributeService.GetAllAsync();

            ViewBag.ProductAttributes = productAttributes.Select(m => new SelectListItem()
            {
                Text = m.Name,
                Value = m.Id.ToString()
            }).ToList();
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(ProductAttributeValueModel model)
        {
            if (model.Id > 0)
            {
                var updateDto = _mapper.Map<UpdateProductAttributeValueDto>(model);
                await _productAttributeValueService.UpdateAsync(updateDto);
                return Json(new { success = true });
            }

            var createDto = _mapper.Map<CreateProductAttributeValueDto>(model);
            await _productAttributeValueService.AddAsync(createDto);
            return Json(new { success = true });
        }

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
