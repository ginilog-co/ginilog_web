
using Genilog_WebApi.Model.LogisticsModel;

namespace Genilog_WebApi.Repository.LogisticsRepo
{
    public interface ILogisticsRepository
    {
        Task<IEnumerable<LogisticsDataModel>> GetAllAsync();
        Task<LogisticsDataModel> GetAsync(Guid id);
        Task<LogisticsDataModel> AddAsync(LogisticsDataModel region);
        Task<LogisticsDataModel> DeleteAsync(Guid id);
        Task<LogisticsDataModel> UpdateAsync(Guid id, LogisticsDataModel region);

        // Logistics Review
        Task<LogisticsReviewModel> AddLogisticsReviewAsync(LogisticsReviewModel region);
        Task<LogisticsReviewModel> DeleteLogisticsReviewAsync(Guid id);

        // Logistics Chat
        Task<IEnumerable<LogisticsChatModelData>> GetAllLogisticsChatAsync();
        Task<LogisticsChatModelData> AddLogisticsChatAsync(LogisticsChatModelData region);
        Task<LogisticsChatModelData> DeleteLogisticsChatAsync(Guid id);
        Task<LogisticsChatModelData> UpdateLogisticsChatAsync(Guid id, LogisticsChatModelData region);
        Task<LogisticsChatModelData> UpdateLogisticsIsReadChatAsync(Guid id, LogisticsChatModelData region);

        // Gas Order
        Task<IEnumerable<OrderModelData>> GetAllOrderAsync();
        Task<OrderModelData> GetOrderAsync(Guid id);
        Task<OrderModelData> AddOrderAsync(OrderModelData region);
        Task<OrderModelData> DeleteOrderAsync(Guid id);
        Task<OrderModelData> UpdateOrderAsync(Guid id, OrderModelData region);
    }
}
