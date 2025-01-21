using Genilog_WebApi.Model.AdminsModel;
using Genilog_WebApi.Model.WalletModel;
using Microsoft.AspNetCore.SignalR;

namespace Genilog_WebApi.Repository.WalletRepo
{
    public class WalletHubRepository : Hub
    {
        public async Task GetAllPayoutAsync(List<PayoutDataModelDto> admin)
        {
            await Clients.All.SendAsync("GetAllPayout", admin);
        }

        public async Task GetPayoutAsync(Guid id,PayoutDataModelDto admin)
        {
            await Clients.All.SendAsync($"GetPayout{id}", admin);
        }
    }
}
