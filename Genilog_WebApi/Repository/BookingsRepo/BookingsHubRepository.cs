using Genilog_WebApi.Model.BookingsModel;
using Genilog_WebApi.Model.UsersDataModel;
using Microsoft.AspNetCore.SignalR;

namespace Genilog_WebApi.Repository.PlacesRepo
{
    public class BookingsHubRepository : Hub
    {
        public async Task GetAllAirlinesAsync(List<AirlineDataModelDto> admin)
        {
            await Clients.All.SendAsync("GetAllAirlines", admin);
        }

        public async Task GetPlaceAsync(Guid id, AirlineDataModelDto admin)
        {
            await Clients.All.SendAsync($"GetAirlines{id}", admin);
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
