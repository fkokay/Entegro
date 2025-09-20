using Entegro.Application.DTOs.Town;
using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    [Authorize]
    public class TownController : Controller
    {
        private readonly ITownService _townService;

        public TownController(ITownService townService)
        {
            _townService = townService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ModalTownModel model)
        {
            var createDto = new CreateTownDto
            {
                Name = model.TownName,
                CityId = model.CityId,
                Published = model.Published
            };
            await _townService.AddAsync(createDto);

            return RedirectToAction("List", "Countries");
        }
    }
}
