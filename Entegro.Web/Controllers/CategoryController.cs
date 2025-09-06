using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models;
using Entegro.Web.Models.Catalog.Categories;
using Entegro.Web.Models.Content;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Azure.Core.HttpHeader;

namespace Entegro.Web.Controllers
{
    [Authorize]
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
                ParentId = model.ParentCategoryId,
                MediaFileId = model.MediaFileId,
                Description = model.Description,
                MetaDescription = model.MetaDescription,
                MetaTitle = model.MetaTitle,
                DisplayOrder = model.DisplayOrder,
                MetaKeywords = model.MetaKeywords,
                Published = model.Published,
            };
            await _categoryService.CreateCategoryAsync(createDto);

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
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
                Parent = category.Parent == null ? null : new CategoryViewModel()
                {
                    Id = category.Parent.Id,
                    CreatedOn = category.Parent.CreatedOn,
                    UpdatedOn = category.Parent.UpdatedOn,
                    Description = category.Parent.Description,
                    DisplayOrder = category.Parent.DisplayOrder,
                    MetaDescription = category.Parent.MetaDescription,
                    MetaKeywords = category.Parent.MetaKeywords,
                    MetaTitle = category.Parent.MetaTitle,
                    Name = category.Parent.Name,
                    ParentCategoryId = category.Parent.ParentCategoryId,
                    TreePath = category.Parent.TreePath,
                    Published = category.Parent.Published,
                },
                Published = category.Published,
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
                    Folder = category.MediaFile.MediaFolder == null ? null : new MediaFolderViewModel()
                    {
                        Id = category.MediaFile.MediaFolder.Id,
                        Name = category.MediaFile.MediaFolder.Name,
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
                    ParentId = model.ParentCategoryId,
                    Name = model.Name,
                    MediaFileId = model.MediaFileId,
                    Description = model.Description,
                    MetaDescription = model.MetaDescription,
                    MetaTitle = model.MetaTitle,
                    DisplayOrder = model.DisplayOrder,
                    MetaKeywords = model.MetaKeywords,
                    Published = model.Published,
                };
                await _categoryService.UpdateCategoryAsync(updateDto);
                return Json(new { success = true });
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, int chooseType)
        {

            try
            {
                if (chooseType == 1)
                {
                    await _categoryService.DeleteCategoryAsync(id);
                    return Json(new { success = true });
                }
                else
                {
                    await _categoryService.DeleteCategoryAsync(id);
                    return Json(new { success = true });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> CategoryList([FromBody] GridCommand model)
        {
            int pageNumber = model.Start / model.Length;
            int pageSize = model.Length;


            var result = await _categoryService.GetPagedAsync(pageNumber, model.Length);

            return Json(new
            {
                draw = model.Draw,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.TotalCount,
                data = result.Items
            });
        }

        [HttpPost]
        public async Task<IActionResult> AllCategory([FromForm] int page = 1, [FromForm] string? term = null)
        {
            var categoryTree = await _categoryService.GetCategoryTreeAsync(includeHidden: true);
            var categories = categoryTree.Flatten(false);

            var query = categories.SelectAwait(async c => new
            {
                id = c.Id.ToString(),
                text = await _categoryService.GetCategoryPathAsync(c),
            });

            var mainList = await query.AsyncToList();

            return Json(new
            {
                results = mainList,
                pagination = new { more = false }
            });
        }

    }
}
