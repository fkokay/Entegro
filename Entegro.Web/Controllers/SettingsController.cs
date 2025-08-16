using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class SettingsController : Controller
    {
        public SettingsController()
        {

        }

        public IActionResult GeneralCommon()
        {
            return View();
        }

        public IActionResult Erp()
        {
            return View();
        }

        public IActionResult ECommerce()
        {
            return View();
        }

        public IActionResult Marketplace()
        {
            return View();
        }

        public IActionResult Cargo()
        {
            return View();
        }

        public IActionResult EInvoice()
        {
            return View();
        }
    }
}
