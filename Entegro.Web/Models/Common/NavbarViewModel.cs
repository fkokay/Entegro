using Entegro.Domain.Entities.Common;

namespace Entegro.Web.Models.Common
{
    public class NavbarViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RoleName { get; set; }

        public List<NotificationViewModel> Notifications { get; set; } = new List<NotificationViewModel>();
    }
}
