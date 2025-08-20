using Entegro.Application.DTOs.Category;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;
using Entegro.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        }

        public IActionResult Index()
        {
            return List();
        }

        public IActionResult List()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            CategoryViewModel model = new CategoryViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            var createDto = new CreateCategoryDto
            {
                Name = model.Name,
                ParentCategoryId = model.ParentCategoryId,
                MediaFileId = model.MediaFileId,
                Description = model.Description,
                MetaDescription = model.MetaDescription,
                MetaTitle = model.MetaTitle,
                DisplayOrder = model.DisplayOrder,
                MetaKeywords = model.MetaKeywords,
            };
            await _categoryService.CreateCategoryAsync(createDto);

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            //var category = await _categoryService.GetCategoryByIdAsync(id);
            var category = await _categoryService.GetByIdWithMediaAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            string formattedParentName = null;
            var formattedCategories = await _categoryService.GetCategoriesFormatTreePathAsync();
            if (category.ParentCategoryId != null)
            {
                var parent = formattedCategories.FirstOrDefault(c => c.Id == category.ParentCategoryId);
                formattedParentName = parent?.FormattedName;
            }
            else
            {
                var self = formattedCategories.FirstOrDefault(c => c.Id == category.Id);
                formattedParentName = self?.FormattedName ?? category.Name;
            }
            var size = category.MediaFile?.Size;
            var categoryModel = new CategoryViewModel
            {
                Id = category.Id,
                CreatedOn = category.CreatedOn,
                UpdatedOn = category.UpdatedOn,
                Description = category.Description,
                DisplayOrder = category.DisplayOrder,
                MetaDescription = category.MetaDescription,
                MetaKeywords = category.MetaKeywords,
                MetaTitle = category.MetaTitle,
                Name = category.Name,
                ParentCategoryId = category.ParentCategoryId,
                TreePath = category.TreePath,
                ParentCategoryFormattedName = formattedParentName,
                MediaFileId = category.MediaFileId,
                MediaFile = category.MediaFile == null ? null : new MediaFileViewModel()
                {
                    Alt = category.MediaFile.Alt,
                    CreatedOn = category.MediaFile.CreatedOn,
                    Deleted = category.MediaFile.Deleted,
                    Extension = category.MediaFile.Extension,
                    FolderId = category.MediaFile.FolderId,
                    Height = category.MediaFile.Height,
                    Id = category.MediaFile.Id,
                    IsTransient = category.MediaFile.IsTransient,
                    MediaType = category.MediaFile.MediaType,
                    Metadata = category.MediaFile.Metadata,
                    MimeType = category.MediaFile.MimeType,
                    Name = category.MediaFile.Name,
                    PixelSize = category.MediaFile.PixelSize,
                    Size = category.MediaFile.Size,
                    Title = category.MediaFile.Title,
                    UpdatedOn = category.MediaFile.UpdatedOn,
                    Width = category.MediaFile.Width,
                    Folder = category.MediaFile.Folder == null ? null : new MediaFolderViewModel()
                    {
                        Id = category.MediaFile.Folder.Id,
                        Name = category.MediaFile.Folder.Name,
                    }
                }

            };


            return View(categoryModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var updateDto = new UpdateCategoryDto
                {
                    Id = model.Id,
                    ParentCategoryId = model.ParentCategoryId,
                    Name = model.Name,
                    MediaFileId = model.MediaFileId,
                    Description = model.Description,
                    MetaDescription = model.MetaDescription,
                    MetaTitle = model.MetaTitle,
                    DisplayOrder = model.DisplayOrder,
                    MetaKeywords = model.MetaKeywords,
                };
                await _categoryService.UpdateCategoryAsync(updateDto);
                return Json(new { success = true });
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, int chooseType)
        {
            if (chooseType == 1)
            {
                var isSuccess = await _categoryService.DeleteCategoryAndChildrenAsync(id);
                if (isSuccess)
                {
                    return Json(new { success = true });
                }
            }
            var isSuccess2 = await _categoryService.DeleteCategoryAndReassignChildrenAsync(id);
            if (isSuccess2)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Silinecek Kategori Bulunamadı" });
        }


        [HttpPost]
        public async Task<IActionResult> CategoryList([FromBody] GridCommand model)
        {
            int pageNumber = model.Start / model.Length;
            int pageSize = model.Length;


            var result = await _categoryService.GetCategoriesAsync(pageNumber, model.Length);

            return Json(new
            {
                draw = model.Draw,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.TotalCount,
                data = result.Items
            });
        }

        [HttpPost]
        public async Task<IActionResult> AllCategory([FromForm] int page = 1, [FromForm] string? term = null, CancellationToken ct = default)
        {
            const int pageSize = 20;
            var data = await _categoryService.GetCategoriesForSelect2Async(term, page, pageSize, ct);
            return Json(data);
        }

    }
}
