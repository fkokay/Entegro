using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Components
{
    public class NavbarViewComponent :ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
