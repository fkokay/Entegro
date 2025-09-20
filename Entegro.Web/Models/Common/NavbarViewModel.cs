using Entegro.Domain.Entities.Common;

namespace Entegro.Web.Models.Common
{
    public class NavbarModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RoleName { get; set; }

        public List<NotificationModel> Notifications { get; set; } = new List<NotificationModel>();
    }
}
