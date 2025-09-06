using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class ImportController : Controller
    {
        public IActionResult Excel()
        {
            return View();
        }

        public IActionResult Xml()
        {
            return View();
        }
    }
}
