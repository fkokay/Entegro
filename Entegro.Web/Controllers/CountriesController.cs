using Entegro.Application.DTOs.Country;
using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class CountriesController : Controller
    {
        private readonly ICountryService _countryService;

        public CountriesController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        public IActionResult List()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            CountryViewModel model = new CountryViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CountryViewModel model)
        {
            var createDto = new CreateCountryDto
            {
                Name = model.Name,
                DisplayOrder = model.DisplayOrder,
                Published = model.Published
            };
            await _countryService.AddAsync(createDto);

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var country = await _countryService.GetByIdAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            var countryModel = new CountryViewModel
            {
                Id = country.Id,
                Name = country.Name,
                Published = country.Published,
                DisplayOrder = country.DisplayOrder,
                Cities = country.Cities.Select(c => new CityViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Published = c.Published,
                    CountryId = c.CountryId,
                }).ToList()
            };

            return View(countryModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateCountryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var updateDto = new UpdateCountryDto
                {
                    Id = model.Id,
                    Name = model.Name,
                    DisplayOrder = model.DisplayOrder,
                    Published = model.Published
                };
                await _countryService.UpdateAsync(updateDto);
                return Json(new { success = true });
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var isSuccess = await _countryService.DeleteAsync(id);
            if (isSuccess)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Silinecek Ülke Bulunamadı" });
        }

        [HttpPost]
        public async Task<IActionResult> CountryList([FromBody] GridCommand model)
        {

            int pageNumber = model.Start / model.Length;
            int pageSize = model.Length;


            var result = await _countryService.GetAllAsync(pageNumber, model.Length);

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
