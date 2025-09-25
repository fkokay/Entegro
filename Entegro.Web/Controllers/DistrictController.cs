using Entegro.Application.DTOs.District;
using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models.Common;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    [Authorize]
    public class DistrictController : Controller
    {
        private readonly IDistrictService _districtService;
        private readonly IMapper _mapper;

        public DistrictController(IDistrictService districtService, IMapper mapper)
        {
            _districtService = districtService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> CreateOrUpdateDistrict(int id, int townId)
        {
            if (id == 0)
            {
                DistrictModel model = new DistrictModel();
                model.TownId = townId;
                return PartialView("_CreateOrUpdateDistrictPartial", model);
            }
            var district = await _districtService.GetByIdAsync(id);
            if (district == null)
            {
                return NotFound();
            }

            var mappedDistrict = _mapper.Map<DistrictModel>(district);
            return PartialView("_CreateOrUpdateDistrictPartial", mappedDistrict);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateDistrict(DistrictModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreateOrUpdateDistrictPartial", model);

            if (model.Id == 0)
            {
                var createMappedDistrict = _mapper.Map<CreateDistrictDto>(model);
                await _districtService.AddAsync(createMappedDistrict);
                return RedirectToAction("List", "Countries");
            }

            var mappedDistrict = _mapper.Map<UpdateDistrictDto>(model);
            await _districtService.UpdateAsync(mappedDistrict);
            return RedirectToAction("List", "Countries");
        }
    }
}
