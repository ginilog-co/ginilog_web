using Genilog_WebApi.Model.LogisticsModel;

namespace Genilog_WebApi.Repository.LogisticsRepo
{
    public interface IRidersRepository
    {
        Task<IEnumerable<RidersModelData>> GetAllAsync();
        Task<RidersModelData> GetAsync(Guid id);
        Task<RidersModelData> AddAsync(RidersModelData region);
        Task<RidersModelData> DeleteAsync(Guid id);
        Task<RidersModelData> UpdateAsync(Guid id, RidersModelData region);

        // Riders Review
        Task<RidersReviewModel> AddRidersReviewAsync(RidersReviewModel region);
        Task<RidersReviewModel> DeleteRidersReviewAsync(Guid id);

        // Riders Chat
        Task<IEnumerable<RidersChatModelData>> GetAllRidersChatAsync();
        Task<RidersChatModelData> AddRidersChatAsync(RidersChatModelData region);
        Task<RidersChatModelData> DeleteRidersChatAsync(Guid id);
        Task<RidersChatModelData> UpdateRidersChatAsync(Guid id, RidersChatModelData region);
        Task<RidersChatModelData> UpdateRidersIsReadChatAsync(Guid id, RidersChatModelData region);

        // Gas Order
        Task<IEnumerable<OrderModelData>> GetAllOrderAsync();
        Task<OrderModelData> GetOrderAsync(Guid id);
        Task<OrderModelData> AddOrderAsync(OrderModelData region);
        Task<OrderModelData> DeleteOrderAsync(Guid id);
        Task<OrderModelData> UpdateOrderAsync(Guid id, OrderModelData region);
    }
}
