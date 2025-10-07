using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.SpecificationAttribute;
using Entegro.Application.DTOs.SpecificationAttributeOption;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Web.Models.Catalog.SpecificationAttributeOptions;
using Entegro.Web.Models.Catalog.SpecificationAttributes;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class SpecificationAttributeController : Controller
    {
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ISpecificationAttributeOptionService _specificationAttributeOptionService;
        public SpecificationAttributeController(ISpecificationAttributeService specificationAttributeService, ISpecificationAttributeOptionService specificationAttributeOptionService)
        {
            _specificationAttributeService = specificationAttributeService ?? throw new ArgumentNullException(nameof(specificationAttributeService));
            _specificationAttributeOptionService = specificationAttributeOptionService ?? throw new ArgumentNullException(nameof(specificationAttributeOptionService));
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List()
        {
            return View();
        }


        #region SpecificationAttribute

        [HttpPost]
        public async Task<IActionResult> AllSpecificationAttribute([FromForm] int page = 1, [FromForm] string term = "")
        {
            var specs = await _specificationAttributeService.GetAllAsync(page, term);

            var query = specs.Items.Select(c => new
            {
                id = c.Id.ToString(),
                text = c.Name,
                specificationAttributeOptions = c.SpecificationAttributeOptions.Select(o => new
                {
                    id = o.Id,
                    text = o.Name
                }).ToList()
            });

            var mainList = query.ToList();
            return Json(new
            {
                results = mainList,
                pagination = new { more = specs.HasNextPage }
            });
        }

        [HttpGet]
        public IActionResult SpecificationAttributeCreatePopup()
        {
            return PartialView("_SpecificationAttributeCreatePopup", new SpecificationAttributeModel());
        }

        [HttpPost]
        public async Task<IActionResult> SpecificationAttributeCreate([FromBody] SpecificationAttributeModel model)
        {
            await _specificationAttributeService.AddAsync(new CreateSpecificationAttributeDto
            {
                Name = model.Name
            });
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var specificationAttributeDto = await _specificationAttributeService.GetByIdAsync(id);
            var specificationAttributeModel = new SpecificationAttributeModel
            {
                Id = specificationAttributeDto.Id,
                Name = specificationAttributeDto.Name,
                SpecificationAttributeOptions = specificationAttributeDto.SpecificationAttributeOptions == null ? null :
                    specificationAttributeDto.SpecificationAttributeOptions.Select(option => new SpecificationAttributeOptionModel
                    {
                        Id = option.Id,
                        Name = option.Name,
                        DisplayOrder = option.DisplayOrder
                    }).ToList()
            };
            return View(specificationAttributeModel);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(SpecificationAttributeModel model)
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
        #endregion

        #region SpecificationAttributeOption
        [HttpGet]
        public IActionResult SpecificationAttributeOptionCreatePopup(int id)
        {
            SpecificationAttributeOptionModel model = new SpecificationAttributeOptionModel();
            model.SpecificationAttributeId = id;
            return PartialView("_SpecificationAttributeOptionCreatePopup", model);
        }
        [HttpPost]
        public async Task<IActionResult> SpecificationAttributeOptionCreate([FromBody] SpecificationAttributeOptionModel model)
        {
            await _specificationAttributeOptionService.AddAsync(new CreateSpecificationAttributeOptionDto
            {
                DisplayOrder = model.DisplayOrder,
                Name = model.Name,
                SpecificationAttributeId = model.SpecificationAttributeId
            });
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> SpecificationAttributeOptionDelete(int id)
        {
            try
            {
                await _specificationAttributeOptionService.DeleteAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion


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


        [HttpPost]
        public async Task<IActionResult> SpecificationAttributeValueList([FromBody] GridCommand gridCommand, int attributeId)
        {
            var result = await _specificationAttributeOptionService.GetPagedAsync(gridCommand, attributeId);
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
