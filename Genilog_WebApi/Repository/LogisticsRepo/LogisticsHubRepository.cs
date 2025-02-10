using Genilog_WebApi.Model.LogisticsModel;
using Microsoft.AspNetCore.SignalR;

namespace Genilog_WebApi.Repository.LogisticsRepo
{
    public class LogisticsHubRepository : Hub
    {

        public async Task GetCompanyAsync(object admin)
        {
            await Clients.All.SendAsync("GetCompany", admin);
        }

        public async Task GetRiderAsync(object admin)
        {
            await Clients.All.SendAsync("GetRider", admin);
        }

        public async Task JoinOrderTracking(string orderId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, orderId);
            Console.WriteLine($"Connection {Context.ConnectionId} joined order {orderId}");
        }

        public async Task SendOrder(string orderId, object newOrder)
        {
            await Clients.Group(orderId).SendAsync("ORDER", newOrder);
        }
    }
}
