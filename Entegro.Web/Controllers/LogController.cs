using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class LogController : Controller
    {
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
