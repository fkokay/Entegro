using Entegro.Domain.Enums;
using Entegro.Web.Models.Platform.Identity;

namespace Entegro.Web.Models.Common
{
    public class NotificationModel
    {
        public int Id { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime NotificationDate { get; set; }
        public bool IsRead { get; set; }
        public int? UserId { get; set; }
        public UserModel User { get; set; }
    }
}
