using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Components
{
    public class FooterViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
