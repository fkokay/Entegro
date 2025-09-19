using Entegro.Application.DTOs.User;
using Entegro.Domain.Enums;

namespace Entegro.Application.DTOs.Notification
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime NotificationDate { get; set; }
        public bool IsRead { get; set; }
        public int? UserId { get; set; }
        public UserDto User { get; set; }
    }
}
