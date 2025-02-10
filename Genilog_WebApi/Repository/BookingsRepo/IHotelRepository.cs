using Genilog_WebApi.Model.BookingsModel;
namespace Genilog_WebApi.Repository.PlacesRepo
{
    public interface IHotelRepository
    {
        Task<IEnumerable<HotelDataModel>> GetAllAsync();
        Task<HotelDataModel> GetAsync(Guid id);
        Task<HotelDataModel> AddAsync(HotelDataModel region);
        Task<HotelDataModel> DeleteAsync(Guid id);
        Task<HotelDataModel> UpdateAsync(Guid id, HotelDataModel region);

        Task<HotelImages> AddHotelImageAsync(HotelImages region);
        Task<HotelImages> DeleteHotelImageAsync(Guid id);

        Task<HotelFacilities> AddHotelFacilitiesAsync(HotelFacilities region);
        Task<HotelFacilities> DeleteHotelFacilitiesAsync(Guid id);

        Task<HotelReviewModel> AddHotelReviewAsync(HotelReviewModel region);
        Task<HotelReviewModel> DeleteHotelReviewAsync(Guid id);

        // Hotel Chat
        Task<IEnumerable<HotelChatModel>> GetAllHotelChatAsync();
        Task<HotelChatModel> AddHotelChatAsync(HotelChatModel region);
        Task<HotelChatModel> DeleteHotelChatAsync(Guid id);
        Task<HotelChatModel> UpdateHotelChatAsync(Guid id, HotelChatModel region);
        Task<HotelChatModel> UpdateHotelIsReadChatAsync(Guid id, HotelChatModel region);
    }
}
