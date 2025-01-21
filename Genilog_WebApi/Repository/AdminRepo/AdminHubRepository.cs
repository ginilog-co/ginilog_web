using Genilog_WebApi.Model.AdminsModel;
using Microsoft.AspNetCore.SignalR;

namespace Genilog_WebApi.Repository.AdminRepo
{
    public class AdminHubRepository : Hub
    {
        public async Task GetAllAdmin(List<AdminModelTableDto>  admin) 
        {
            await Clients.All.SendAsync("GetAllAdmin", admin);
        }

        public async Task GetAdmin(Guid id, AdminModelTableDto admin)
        {
            await Clients.All.SendAsync($"GetAdmin{id}", admin);
        }
    }
}
