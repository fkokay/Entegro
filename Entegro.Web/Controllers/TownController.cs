using Entegro.Application.DTOs.Town;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Web.Models.Common;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    [Authorize]
    public class TownController : Controller
    {
        private readonly ITownService _townService;
        private readonly IMapper _mapper;

        public TownController(ITownService townService, IMapper mapper)
        {
            _townService = townService;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateOrUpdateTown(int id, int cityId)
        {
            if (id == 0)
            {
                TownModel model = new TownModel();
                model.CityId = cityId;
                return PartialView("_CreateOrUpdateTownPartial", model);
            }
            var town = await _townService.GetByIdAsync(id);
            if (town == null)
            {
                return NotFound();
            }

            var mappedTown = _mapper.Map<TownModel>(town);
            return PartialView("_CreateOrUpdateTownPartial", mappedTown);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateTown(TownModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreateOrUpdateTownPartial", model);

            if (model.Id == 0)
            {
                var createMappedTown = _mapper.Map<CreateTownDto>(model);
                await _townService.AddAsync(createMappedTown);
                return RedirectToAction("List", "Countries");
            }

            var mappedTown = _mapper.Map<UpdateTownDto>(model);
            await _townService.UpdateAsync(mappedTown);
            return RedirectToAction("List", "Countries");
        }


        [HttpGet]
        public async Task<IActionResult> GetTowns(int cityId)
        {
            var townDto = await _townService.GetByCityIdAsync(cityId);
            if (townDto == null)
                return NotFound();
            var town = _mapper.Map<List<TownModel>>(townDto);
            return PartialView("_TownListPartial", town);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTown(int id)
        {
            try
            {
                await _townService.DeleteAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
