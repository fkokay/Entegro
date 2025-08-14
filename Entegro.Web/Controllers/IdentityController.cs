using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class IdentityController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
