using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class OrderDetailController : Controller
    {
        public IActionResult Detail()
        {
            return View();
        }
    }
}
