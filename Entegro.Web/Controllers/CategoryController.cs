using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Common;
using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models.Catalog.Categories;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        public CategoryController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
            CategoryModel model = new CategoryModel()
            {
                Published = true
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryModel model)
        {
            var createDto = _mapper.Map<CreateCategoryDto>(model);
            await _categoryService.AddAsync(createDto);

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
            var model = _mapper.Map<CategoryModel>(category);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryModel model)
        {
            if (ModelState.IsValid)
            {
                var updateDto = _mapper.Map<UpdateCategoryDto>(model);
                await _categoryService.UpdateAsync(updateDto);
                return Json(new { success = true });
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, int chooseType)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(id, chooseType == 1);
                return Json(new { success = true });
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
            var categoryTree = await _categoryService.GetCategoryTreeAsync(includeHidden: false);
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
