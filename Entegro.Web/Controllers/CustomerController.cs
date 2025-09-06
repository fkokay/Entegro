using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Edit(int id)
        {
            ViewBag.CustomerId = id;
            return View();
        }


        public IActionResult List()
        {
            return View();
        }
    }
}
