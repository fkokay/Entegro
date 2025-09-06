using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class ReturnRequestController : Controller
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
