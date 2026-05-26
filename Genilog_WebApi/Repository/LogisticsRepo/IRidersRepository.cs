using Genilog_WebApi.Model;
using Genilog_WebApi.Model.GeneraModel;
using Genilog_WebApi.Model.LogisticsModel;

namespace Genilog_WebApi.Repository.LogisticsRepo
{
    public interface IRidersRepository
    {
        Task<IEnumerable<RidersModelData>> GetAllAsync();
        Task<PageModel<RidersModelDataDto>> GetAllPaginatedRidersAsync(FilterLocationData filter);
        Task<RidersModelData> GetAsync(Guid id);
        Task<RidersModelData> AddAsync(RidersModelData region);
        Task<RidersModelData> DeleteAsync(Guid id);
        Task<RidersModelData> UpdateAsync(Guid id, RidersModelData region);

        // Riders Review
        Task<RidersReviewModel> AddRidersReviewAsync(RidersReviewModel region);
        Task<IEnumerable<RidersReviewModel>> GetRidersReviewByIdAsync(Guid riderId);
        Task<RidersReviewModel> DeleteRidersReviewAsync(Guid id);

        // Riders Chat
        Task<IEnumerable<RidersChatModelData>> GetAllRidersChatAsync();
        Task<IEnumerable<RidersChatModelData>> GetAllRidersChatByIdAsync(Guid riderId);
        Task<RidersChatModelData> AddRidersChatAsync(RidersChatModelData region);
        Task<RidersChatModelData> DeleteRidersChatAsync(Guid id);
        Task<RidersChatModelData> UpdateRidersChatAsync(Guid id, RidersChatModelData region);
        Task<RidersChatModelData> UpdateRidersIsReadChatAsync(Guid id, RidersChatModelData region);

      
    }
}
