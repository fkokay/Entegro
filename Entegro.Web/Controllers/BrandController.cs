using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Services.Base;
using Entegro.Web.Models.Catalog.Brands;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    [Authorize]
    public class BrandController : Controller
    {
        private readonly IBrandService _brandService;
        private readonly IMapper _mapper;
        public BrandController(IBrandService brandService, IMapper mapper)
        {
            _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost]
        public async Task<IActionResult> AllBrand([FromForm] int page = 1, [FromForm] string term = "")
        {
            var brands = await _brandService.GetBrandsAsync(page, term);

            var query = brands.Items.Select(c => new
            {
                id = c.Id.ToString(),
                text = c.Name,
            });

            var mainList = query.ToList();

            return Json(new
            {
                results = mainList,
                pagination = new { more = brands.HasNextPage }
            });
        }

        public IActionResult Index()
        {
            return List();
        }

        public IActionResult List()
        {
            return View();
        }

        public IActionResult Create()
        {
            BrandModel model = new BrandModel();
            model.DisplayOrder = 0;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BrandModel model)
        {
            if (ModelState.IsValid)
            {
                var createDto = _mapper.Map<CreateBrandDto>(model);

                await _brandService.AddAsync(createDto);
                return Json(new { success = true });
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var brand = await _brandService.GetByIdAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            var model = _mapper.Map<BrandModel>(brand);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BrandModel model)
        {
            if (ModelState.IsValid)
            {
                var updateDto = _mapper.Map<UpdateBrandDto>(model);
                await _brandService.UpdateAsync(updateDto);

                return Json(new { success = true });
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _brandService.DeleteAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BrandList([FromBody] GridCommand gridCommand)
        {
            var result = await _brandService.GetPagedAsync(gridCommand);

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
