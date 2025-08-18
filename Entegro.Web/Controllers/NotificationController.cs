using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class NotificationController : Controller
    {
        public IActionResult Index()
        {
            return List();
        }

        public IActionResult List()
        {
            return View();
        }
    }
}
