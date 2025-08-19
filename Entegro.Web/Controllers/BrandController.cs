using Entegro.Application.DTOs.Brand;
using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class BrandController : Controller
    {
        private readonly IBrandService _brandService;
        public BrandController(IBrandService brandService)
        {
            _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
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
            BrandViewModel model = new BrandViewModel();
            model.DisplayOrder = 0;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BrandViewModel model)
        {
            if (ModelState.IsValid)
            {
                var createDto = new CreateBrandDto
                {
                    Name = model.Name,
                    Description = model.Description,
                    MetaDescription = model.MetaDescription,
                    MetaTitle = model.MetaTitle,
                    DisplayOrder = model.DisplayOrder,
                    MetaKeywords = model.MetaKeywords,
                    MediaFileId = model.MediaFileId
                };

                await _brandService.CreateBrandAsync(createDto);
                return Json(new { success = true });
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var brand = await _brandService.GetByIdWithMediaAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            var size = brand.MediaFile?.Size;
            var brandModel = new BrandViewModel
            {
                Id = brand.Id,
                Name = brand.Name,
                Description = brand.Description,
                MetaDescription = brand.MetaDescription,
                MetaTitle = brand.MetaTitle,
                DisplayOrder = brand.DisplayOrder,
                MetaKeywords = brand.MetaKeywords,
                MediaFileId = brand.MediaFileId,
                MediaFileName = brand.MediaFile?.Name,
                MediaFileSize = size.HasValue ? (int)size.Value : 0,
                MediaFileUrl = $"/uploads/Brand/{brand.MediaFile?.Name}",
            };
            return View(brandModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BrandViewModel model)
        {
            if (ModelState.IsValid)
            {
                var updateDto = new UpdateBrandDto
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description,
                    MetaDescription = model.MetaDescription,
                    MetaTitle = model.MetaTitle,
                    DisplayOrder = model.DisplayOrder,
                    MetaKeywords = model.MetaKeywords,
                    MediaFileId = model.MediaFileId
                };
                await _brandService.UpdateBrandAsync(updateDto);
                return Json(new { success = true });
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var isSuccess = await _brandService.DeleteBrandAsync(id);
            if (isSuccess)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Silinecek Marka Bulunamadı" });
        }

        [HttpPost]
        public async Task<IActionResult> BrandList([FromBody] GridCommand model)
        {

            int pageNumber = model.Start / model.Length;
            int pageSize = model.Length;


            var result = await _brandService.GetBrandsAsync(pageNumber, model.Length);

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
