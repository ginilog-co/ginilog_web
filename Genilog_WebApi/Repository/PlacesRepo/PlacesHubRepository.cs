using Genilog_WebApi.Model.PlacesModel;
using Genilog_WebApi.Model.UsersDataModel;
using Microsoft.AspNetCore.SignalR;

namespace Genilog_WebApi.Repository.PlacesRepo
{
    public class PlacesHubRepository : Hub
    {
        public async Task GetAllPlacesAsync(List<PlacesDataModelDto> admin)
        {
            await Clients.All.SendAsync("GetAllPlaces", admin);
        }

        public async Task GetPlaceAsync(Guid id, PlacesDataModelDto admin)
        {
            await Clients.All.SendAsync($"GetPlaces{id}", admin);
        }
        public async Task GetAllHotelAsync(List<HotelDataModelDto> admin)
        {
            await Clients.All.SendAsync("GetAllHotel", admin);
        }

        public async Task GetHotelAsync(Guid id, HotelDataModelDto admin)
        {
            await Clients.All.SendAsync($"GetHotel{id}", admin);
        }
    }
}
