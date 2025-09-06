using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
