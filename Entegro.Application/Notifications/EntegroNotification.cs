using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Notifications
{
    public static class EntegroNotification
    {
        public static async Task SendNotification(string message)
        {
            var connection = new HubConnectionBuilder()
          .WithUrl("https://localhost:4000/notificationHub") // Hub’ın adresi
          .Build();

            await connection.StartAsync();

            await connection.InvokeAsync("SendNotification", message);

            await connection.StopAsync();
        }
    }
}
