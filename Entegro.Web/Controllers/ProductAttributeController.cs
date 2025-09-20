using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.ProductAttribute;
using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models.Catalog.Attributes;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    [Authorize]
    public class ProductAttributeController : Controller
    {
        private readonly IProductAttributeService _productAttributeService;
        private readonly IMapper _mapper;
        public ProductAttributeController(IProductAttributeService productAttributeService,IMapper mapper)
        {
            _productAttributeService = productAttributeService ?? throw new ArgumentNullException(nameof(productAttributeService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
            var productAttribute = await _productAttributeService.GetByIdAsync(id);
            if (productAttribute == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<ProductAttributeModel>(productAttribute);
            return PartialView("_CreateOrUpdate", model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(ProductAttributeModel model)
        {
            if (model.Id > 0)
            {
                var updateDto = _mapper.Map<UpdateProductAttributeDto>(model);
                await _productAttributeService.UpdateAsync(updateDto);
                return Json(new { success = true });
            }

            var createDto = _mapper.Map<CreateProductAttributeDto>(model);
            await _productAttributeService.AddAsync(createDto);
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
