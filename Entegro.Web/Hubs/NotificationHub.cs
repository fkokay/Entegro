using Entegro.Application.DTOs.Notification;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Enums;
using Microsoft.AspNetCore.SignalR;

namespace Entegro.Web.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly INotificationService _notificationService;
        public NotificationHub(INotificationService notificationService) 
        {
            _notificationService = notificationService;
        }
        public async Task SendNotification(NotificationType type,string title,string message)
        {
            CreateNotificationDto createNotification =new CreateNotificationDto();
            createNotification.Title = title;
            createNotification.Message = message;
            createNotification.Type = type;
            createNotification.NotificationDate = DateTime.Now;
            createNotification.IsRead = false;
            createNotification.UserId = null;

            await _notificationService.AddAsync(createNotification);


            await Clients.All.SendAsync("ReceiveNotification",(int)type, title, message);
        }
    }
}
