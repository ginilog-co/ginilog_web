using Microsoft.AspNetCore.SignalR;

namespace Genilog_WebApi.Repository.NotificationRepo
{
    public class NotificationHub:Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage",user, message);
        }
        public async Task JoinNotification(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            Console.WriteLine($"Connection {Context.ConnectionId} joined Notification {userId}");
        }

        public async Task SendNotification(string userId, object notify)
        {
            await Clients.Group(userId).SendAsync("NOTIFICATION", notify);
        }
    }
}
