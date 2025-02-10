using Microsoft.AspNetCore.SignalR;

namespace Genilog_WebApi.Repository.UserRepo
{
    public class UserHubRepository : Hub
    {

        public async Task GetUserAsync( object admin)
        {
            await Clients.All.SendAsync("GetUser", admin);
        }
    }
}
