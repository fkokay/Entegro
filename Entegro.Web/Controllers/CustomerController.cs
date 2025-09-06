using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
