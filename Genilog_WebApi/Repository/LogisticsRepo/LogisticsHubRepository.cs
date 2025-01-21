using Genilog_WebApi.Model.LogisticsModel;
using Microsoft.AspNetCore.SignalR;

namespace Genilog_WebApi.Repository.LogisticsRepo
{
    public class LogisticsHubRepository : Hub
    {
        public async Task GetAllCompanyAsync(List<CompanyModelDataDto> admin)
        {
            await Clients.All.SendAsync("GetAllCompany", admin);
        }

        public async Task GetCompanyAsync(Guid id, CompanyModelDataDto admin)
        {
            await Clients.All.SendAsync($"GetCompany{id}", admin);
        }
        public async Task GetAllRiderAsync(List<RidersModelDataDto> admin)
        {
            await Clients.All.SendAsync("GetAllRider", admin);
        }

        public async Task GetRiderAsync(Guid id, RidersModelDataDto admin)
        {
            await Clients.All.SendAsync($"GetRider{id}", admin);
        }
    }
}
