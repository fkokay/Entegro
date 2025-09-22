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



        [HttpPost]
        public async Task<IActionResult> Create(ModalCityModel model)
        {

            if (ModelState.IsValid)
            {
                var createDto = _mapper.Map<CreateCityDto>(model);
                await _cityService.AddAsync(createDto);
                return RedirectToAction("List", "Countries");
            }
            return View(model);
        }
    }
}
