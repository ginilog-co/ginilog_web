using Genilog_WebApi.Model.AdminsModel;
using Microsoft.AspNetCore.SignalR;

namespace Genilog_WebApi.Repository.AdminRepo
{
    public class AdminHubRepository : Hub
    {
        public async Task GetAdmin(object admin)
        {
            await Clients.All.SendAsync("GetAdmin", admin);
        }
    }
}
