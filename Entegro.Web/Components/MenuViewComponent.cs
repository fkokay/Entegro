using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Components
{
    public class MenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
