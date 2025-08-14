using Entegro.Application.DTOs.Category;
using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models;
using Entegro.Web.Models.CategoryViewModels;
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
            // Doğrudan servis çağrısı
            await _categoryService.CreateCategoryAsync(new CreateCategoryDto
            {
                Name = model.Name,
                ParentCategoryId = model.ParentCategoryId,
                Description = model.Description,
                MetaDescription = model.MetaDescription,
                MetaTitle = model.MetaTitle,
                DisplayOrder = model.DisplayOrder,
                MetaKeywords = model.MetaKeywords,
            });

            // AJAX için JSON formatında yanıt dön
            return Json(new { success = true });
        }


        [HttpPost]
        public async Task<IActionResult> CategoryList([FromBody] DatatableData model)
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
        public async Task<IActionResult> AllCateogry(int page,string term)
        {
            var result = await _categoryService.GetCategoriesFormatTreePathAsync();

            return Json(new { results = result.Select(c => new 
            {
                id = c.Id,
                text = c.FormattedName
            }), pagination = new { more = false } });
        }
    }
}
