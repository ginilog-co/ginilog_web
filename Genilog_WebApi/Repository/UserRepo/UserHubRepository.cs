using Genilog_WebApi.Model.UsersDataModel;
using Microsoft.AspNetCore.SignalR;

namespace Genilog_WebApi.Repository.UserRepo
{
    public class UserHubRepository : Hub
    {
        public async Task GetAllUserAsync(List<UsersDataModelTableDto> admin)
        {
            await Clients.All.SendAsync("GetAllUser", admin);
        }

        public async Task GetUserAsync(Guid id, UsersDataModelTableDto admin)
        {
            await Clients.All.SendAsync($"GetUser{id}", admin);
        }
    }
}
