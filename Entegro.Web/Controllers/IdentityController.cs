using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models.IdentityViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class IdentityController : Controller
    {
        private readonly IUserService _userService;

        public IdentityController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var user = await _userService.GetByEmailAndPasswordAsync(model.Email, model.Password);
            if (user is not null)
                return RedirectToAction("List", "Brand");

            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            //mail için servis gerekiyor

            return View(model);
        }



        [HttpGet]
        public IActionResult Logout()
        {
            return RedirectToAction("Login", "Identity");
        }
    }
}
