using Entegro.Web.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Entegro.Application.Interfaces.Services;
using System.Threading.Tasks;
using MapsterMapper;

namespace Entegro.Web.Components
{
    public class NavbarViewComponent : ViewComponent
    {
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        public NavbarViewComponent(IUserService userService,INotificationService notificationService,IMapper mapper)
        {
            _userService = userService;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int userId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _userService.GetUserByIdAsync(userId);

            var notifications =await _notificationService.GetAllAsync();
            

            NavbarModel model = new NavbarModel();
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.RoleName = "Admin";
            model.Notifications = _mapper.Map<List<NotificationModel>>(notifications);    


            return View(model);
        }
    }
}
