using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Erp()
        {
            return View();
        }
    }
}
