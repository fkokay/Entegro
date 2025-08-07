using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Components
{
    public class LogoViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
