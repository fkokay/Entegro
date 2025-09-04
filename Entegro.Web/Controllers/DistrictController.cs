using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    [Authorize]
    public class DistrictController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
