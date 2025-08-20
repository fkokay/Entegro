using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class DistrictController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
