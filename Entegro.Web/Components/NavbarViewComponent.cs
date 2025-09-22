using Entegro.Web.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Entegro.Application.Interfaces.Services;
using System.Threading.Tasks;

namespace Entegro.Web.Components
{
    public class NavbarViewComponent : ViewComponent
    {
        private readonly IUserService _userService;
        public NavbarViewComponent(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int userId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _userService.GetUserByIdAsync(userId);

            NavbarModel model = new NavbarModel();
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.RoleName = "Admin";


            return View(model);
        }
    }
}
