using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Components
{
    public class DashboardViewComponent : ViewComponent
    {
        public DashboardViewComponent()
        {

        }

        public IViewComponentResult Invoke(string dashboardType)
        {
            return View(dashboardType);
        }
    }
}
