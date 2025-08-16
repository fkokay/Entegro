using Entegro.Application.DTOs.Brand;
using Entegro.Application.DTOs.User;
using Entegro.Application.Interfaces.Services;
using Entegro.Application.Services;
using Entegro.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        public UserController(IUserService userService,ILogger<UserController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List()
        {
            return View();
        }

        public IActionResult Create()
        {
            UserViewModel model = new UserViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var createDto = new CreateUserDto
                {
                    FirsName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = model.Password,
                    PhoneNumber = model.PhoneNumber,
                    Active = model.Active
                };

                await _userService.CreateUserAsync(createDto);
                return Json(new { success = true });
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var userModle = new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,
                PhoneNumber = user.PhoneNumber,
                Active = user.Active
            };
            return View(userModle);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var updateDto = new UpdateUserDto
                {
                    Id = model.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = model.Password,
                    PhoneNumber = model.PhoneNumber,
                    Active = model.Active
                };
                await _userService.UpdateUserAsync(updateDto);
                return Json(new { success = true });
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var isSuccess = await _userService.DeleteUserAsync(id);
            if (isSuccess)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Silinecek Marka Bulunamadı" });
        }

        [HttpPost]
        public async Task<IActionResult> UserList([FromBody] GridCommand model)
        {

            int pageNumber = model.Start / model.Length;
            int pageSize = model.Length;


            var result = await _userService.GetUsersAsync(pageNumber, model.Length);

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
