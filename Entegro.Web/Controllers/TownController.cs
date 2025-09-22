using Entegro.Application.DTOs.Town;
using Entegro.Application.Interfaces.Services;
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

        [HttpPost]
        public async Task<IActionResult> Create(ModalTownModel model)
        {
            if (ModelState.IsValid)
            {
                var createDto = _mapper.Map<CreateTownDto>(model);
                await _townService.AddAsync(createDto);
                return RedirectToAction("List", "Countries");
            }
            return View(model);
        }
    }
}
