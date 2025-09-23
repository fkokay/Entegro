using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.User;
using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models.Platform.Identity;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;
        public UserController(IUserService userService, ILogger<UserController> logger, IMapper mapper)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper;
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
            UserModel model = new UserModel();
            model.Active = true;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserModel model)
        {

            if (ModelState.IsValid)
            {
                var createDto = _mapper.Map<CreateUserDto>(model);
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

            var model = _mapper.Map<UserModel>(user);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserModel model)
        {
            if (ModelState.IsValid)
            {
                var updateDto = _mapper.Map<UpdateUserDto>(model);
                await _userService.UpdateUserAsync(updateDto);

                return RedirectToAction("List");
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
        public async Task<IActionResult> UserList([FromBody] GridCommand gridCommand)
        {
            var result = await _userService.GetPagedAsync(gridCommand);
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
