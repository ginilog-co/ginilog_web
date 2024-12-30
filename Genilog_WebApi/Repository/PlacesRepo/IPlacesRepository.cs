
using Genilog_WebApi.Model.PlacesModel;

namespace Genilog_WebApi.Repository.PlacesRepo
{
    public interface IPlacesRepository
    {
        Task<IEnumerable<PlacesDataModel>> GetAllAsync();
        Task<PlacesDataModel> GetAsync(Guid id);
        Task<PlacesDataModel> AddAsync(PlacesDataModel region);
        Task<PlacesDataModel> DeleteAsync(Guid id);
        Task<PlacesDataModel> UpdateAsync(Guid id, PlacesDataModel region);

        Task<PlacesDataModel> UpdateTimeSheduleAsync(Guid id, PlacesDataModel region);

        Task<PlaceImages> AddPlaceImageAsync(PlaceImages region);
        Task<PlaceImages> DeletePlaceImageAsync(Guid id);

        Task<PlaceFacilities> AddPlaceFacilitiesAsync(PlaceFacilities region);
        Task<PlaceFacilities> DeletePlaceFacilitiesAsync(Guid id);

        Task<PlaceWhatToExpect> AddPlaceWhatToExpectAsync(PlaceWhatToExpect region);
        Task<PlaceWhatToExpect> DeletePlaceWhatToExpectAsync(Guid id);

        Task<PlaceReviewModel> AddPlaceReviewAsync(PlaceReviewModel region);
        Task<PlaceReviewModel> DeletePlaceReviewAsync(Guid id);

        // Places Chat
        Task<IEnumerable<PlacesChatModel>> GetAllPlacesChatAsync();
        Task<PlacesChatModel> AddPlacesChatAsync(PlacesChatModel region);
        Task<PlacesChatModel> DeletePlacesChatAsync(Guid id);
        Task<PlacesChatModel> UpdatePlacesChatAsync(Guid id, PlacesChatModel region);
        Task<PlacesChatModel> UpdatePlacesIsReadChatAsync(Guid id, PlacesChatModel region);
    }
}
