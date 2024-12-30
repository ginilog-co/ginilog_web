using Microsoft.AspNetCore.SignalR;

namespace Genilog_WebApi.Repository.NotificationRepo
{
    public class NotificationHub:Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage",user, message);
        }
    }
}
