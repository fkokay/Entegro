using Entegro.Application.Interfaces.Services;
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
    }
}
