using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Country;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Web.Models.Common;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    [Authorize]
    public class CountriesController : Controller
    {
        private readonly ICountryService _countryService;
        private readonly IMapper _mapper;

        public CountriesController(ICountryService countryService, IMapper mapper)
        {
            _countryService = countryService;
            _mapper = mapper;
        }

        public IActionResult List()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            CountryModel model = new CountryModel();
            model.DisplayOrder = 0;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CountryModel model)
        {
            if (ModelState.IsValid)
            {
                var createDto = _mapper.Map<CreateCountryDto>(model);
                await _countryService.AddAsync(createDto);
                return Json(new { success = true });
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var country = await _countryService.GetByIdAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            var model = _mapper.Map<CountryModel>(country);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CountryModel model)
        {

            if (ModelState.IsValid)
            {
                var updateDto = _mapper.Map<UpdateCountryDto>(model);
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
