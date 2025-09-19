using Microsoft.AspNetCore.SignalR;

namespace Entegro.Web.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(int type,string title,string message)
        {
            await Clients.All.SendAsync("ReceiveNotification",type, title, message);
        }
    }
}
