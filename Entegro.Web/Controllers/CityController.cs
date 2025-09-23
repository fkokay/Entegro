using Entegro.Application.DTOs.City;
using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models.Common;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    [Authorize]
    public class CityController : Controller
    {
        private readonly ICityService _cityService;
        private readonly IMapper _mapper;

        public CityController(ICityService cityService, IMapper mapper)
        {
            _cityService = cityService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> CreateOrUpdateCity(int id, int countryId)
        {
            if (id == 0)
            {
                CityModel model = new CityModel();
                model.CountryId = countryId;
                return PartialView("_CreateOrUpdateCityPartial", model);
            }
            var town = await _cityService.GetByIdAsync(id);
            if (town == null)
            {
                return NotFound();
            }

            var mappedCity = _mapper.Map<CityModel>(town);
            return PartialView("_CreateOrUpdateCityPartial", mappedCity);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateCity(CityModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreateOrUpdateCityPartial", model);

            if (model.Id == 0)
            {
                var createMappedCity = _mapper.Map<CreateCityDto>(model);
                await _cityService.AddAsync(createMappedCity);
                return RedirectToAction("List", "Countries");
            }

            var mappedCity = _mapper.Map<UpdateCityDto>(model);
            await _cityService.UpdateAsync(mappedCity);
            return RedirectToAction("List", "Countries");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteCity(int id)
        {
            try
            {
                await _cityService.DeleteAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
