using Entegro.Application.DTOs.Category;
using Entegro.Application.Interfaces.Services;
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
            var category = await _categoryService.GetCategoryByIdAsync(id);
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
                ParentCategoryFormattedName = formattedParentName
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
