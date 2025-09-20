using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models.Catalog.Brands;
using Entegro.Web.Models.Content;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    [Authorize]
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
            BrandModel model = new BrandModel();
            model.DisplayOrder = 0;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BrandModel model)
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
                    MediaFileId = model.MediaFileId,
                    Published = model.Published,
                };

                await _brandService.CreateAsync(createDto);
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
            var size = brand.MediaFile?.Size;
            var brandModel = new BrandModel
            {
                Id = brand.Id,
                Name = brand.Name,
                Description = brand.Description,
                MetaDescription = brand.MetaDescription,
                MetaTitle = brand.MetaTitle,
                DisplayOrder = brand.DisplayOrder,
                Published = brand.Published,
                MetaKeywords = brand.MetaKeywords,
                MediaFileId = brand.MediaFileId,
                MediaFile = brand.MediaFile == null ? null : new MediaFileModel()
                {
                    Alt = brand.MediaFile.Alt,
                    CreatedOn = brand.MediaFile.CreatedOn,
                    Deleted = brand.MediaFile.Deleted,
                    Extension = brand.MediaFile.Extension,
                    FolderId = brand.MediaFile.FolderId,
                    Height = brand.MediaFile.Height,
                    Id = brand.MediaFile.Id,
                    IsTransient = brand.MediaFile.IsTransient,
                    MediaType = brand.MediaFile.MediaType,
                    Metadata = brand.MediaFile.Metadata,
                    MimeType = brand.MediaFile.MimeType,
                    Name = brand.MediaFile.Name,
                    PixelSize = brand.MediaFile.PixelSize,
                    Size = brand.MediaFile.Size,
                    Title = brand.MediaFile.Title,
                    UpdatedOn = brand.MediaFile.UpdatedOn,
                    Width = brand.MediaFile.Width,
                    Folder = brand.MediaFile.Folder == null ? null : new MediaFolderModel()
                    {
                        Id = brand.MediaFile.Folder.Id,
                        Name = brand.MediaFile.Folder.Name,
                    }
                }
            };
            return View(brandModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BrandModel model)
        {
            if (ModelState.IsValid)
            {
                var updateDto = new UpdateBrandDto
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description,
                    MediaFileId = model.MediaFileId == 0 ? null : model.MediaFileId,
                    MetaDescription = model.MetaDescription,
                    MetaTitle = model.MetaTitle,
                    DisplayOrder = model.DisplayOrder,
                    MetaKeywords = model.MetaKeywords,
                    Published = model.Published,
                };
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
