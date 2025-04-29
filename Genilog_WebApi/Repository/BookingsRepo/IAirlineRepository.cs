using Genilog_WebApi.Model.BookingsModel;

namespace Genilog_WebApi.Repository.BookingsRepo
{
    public interface IAirlineRepository
    {
        Task<IEnumerable<AirlineDataModel>> GetAllAsync();
        Task<AirlineDataModel> GetAsync(Guid id);
        Task<AirlineDataModel> AddAsync(AirlineDataModel region);
        Task<AirlineDataModel> DeleteAsync(Guid id);
        Task<AirlineDataModel> UpdateAsync(Guid id, AirlineDataModel region);

        Task<AirCraftList> AddAirCraftListAsync(AirCraftList region);
        Task<AirCraftList> DeleteAirCraftListAsync(Guid id);

        Task<AirlineReviewModel> AddAirlineReviewAsync(AirlineReviewModel region);
        Task<AirlineReviewModel> DeleteAirlineReviewAsync(Guid id);

        Task<AirLineServiceLocation> AddAirLineServiceLocationAsync(AirLineServiceLocation region);
        Task<AirLineServiceLocation> DeleteAirLineServiceLocationAsync(Guid id);

        // Airline Ticket
        Task<IEnumerable<FlightTicketBookModel>> GetAllAirlineFlightTicketAsync();
        Task<FlightTicketBookModel> AddAirlineFlightTicketAsync(FlightTicketBookModel region);
        Task<FlightTicketBookModel> UpdateAirlineFlightTicketAsync(Guid id,FlightTicketBookModel region);
        Task<FlightTicketBookModel> GetAirlineFlightTicketAsync(Guid id);
        Task<FlightTicketBookModel> DeleteAirlineFlightTicketAsync(Guid id);
        // Places Chat
        Task<IEnumerable<AirlineChatModel>> GetAllAirlineChatAsync();
        Task<AirlineChatModel> AddAirlineChatAsync(AirlineChatModel region);
        Task<AirlineChatModel> DeleteAirlineChatAsync(Guid id);
        Task<AirlineChatModel> UpdateAirlineChatAsync(Guid id, AirlineChatModel region);
        Task<AirlineChatModel> UpdateAirlinesIsReadChatAsync(Guid id, AirlineChatModel region);
    }
}
