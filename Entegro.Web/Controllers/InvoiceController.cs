using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class InvoiceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
