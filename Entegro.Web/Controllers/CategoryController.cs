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
            };


            return View(categoryModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateCategoryDto model)
        {
            if (ModelState.IsValid)
            {

                await _categoryService.UpdateCategoryAsync(model);
                return Json(new { success = true });
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var isSuccess = await _categoryService.DeleteCategoryAsync(id);
            if (isSuccess)
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
        public async Task<IActionResult> AllCateogry(int page, string term)
        {
            var result = await _categoryService.GetCategoriesFormatTreePathAsync();

            return Json(new
            {
                results = result.Select(c => new
                {
                    id = c.Id,
                    text = c.FormattedName
                }),
                pagination = new { more = false }
            });
        }
    }
}
