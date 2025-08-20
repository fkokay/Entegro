using Entegro.Application.DTOs.City;
using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class CityController : Controller
    {
        private readonly ICityService _cityService;

        public CityController(ICityService cityService)
        {
            _cityService = cityService;
        }

        public IActionResult Index()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Create(CityViewModel model)
        {
            var createDto = new CreateCityDto
            {
                Name = model.Name,
                CountryId = model.CountryId,
                Published = model.Published
            };
            await _cityService.AddAsync(createDto);

            return Json(new { success = true });
        }
    }
}
