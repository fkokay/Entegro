using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.SpecificationAttribute;
using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models.Catalog.SpecificationAttributeOptions;
using Entegro.Web.Models.Catalog.SpecificationAttributes;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class SpecificationAttributeController : Controller
    {
        private readonly ISpecificationAttributeService _specificationAttributeService;
        public SpecificationAttributeController(ISpecificationAttributeService specificationAttributeService)
        {
            _specificationAttributeService = specificationAttributeService ?? throw new ArgumentNullException(nameof(specificationAttributeService));
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SpecificationAttributeCreatePopup()
        {
            return PartialView("_SpecificationAttributeCreatePopup", new CreateSpecificationAttributeViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> SpecificationAttributeCreate([FromBody] CreateSpecificationAttributeViewModel model)
        {
            await _specificationAttributeService.CreateAsync(new CreateSpecificationAttributeDto
            {
                Name = model.Name
            });
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var specificationAttributeDto = await _specificationAttributeService.GetByIdAsync(id);
            var specificationAttributeViewModel = new SpecificationAttributeViewModel
            {
                Id = specificationAttributeDto.Id,
                Name = specificationAttributeDto.Name,
                SpecificationAttributeOptions = specificationAttributeDto.SpecificationAttributeOptions == null ? null :
                    specificationAttributeDto.SpecificationAttributeOptions.Select(option => new SpecificationAttributeOptionViewModel
                    {
                        Id = option.Id,
                        Name = option.Name,
                        DisplayOrder = option.DisplayOrder
                    }).ToList()
            };
            return View(specificationAttributeViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(UpdateSpecificationAttributeViewModel model)
        {
            await _specificationAttributeService.UpdateAsync(new UpdateSpecificationAttributeDto
            {
                Id = model.Id,
                Name = model.Name
            });
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _specificationAttributeService.DeleteAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SpecificationAttributeList([FromBody] GridCommand gridCommand)
        {
            var result = await _specificationAttributeService.GetPagedAsync(gridCommand);
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
