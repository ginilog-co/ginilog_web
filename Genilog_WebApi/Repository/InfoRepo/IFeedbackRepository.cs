

using Genilog_WebApi.Model.InfoModel;

namespace Genilog_WebApi.Repository.InfoRepo
{
    public interface IFeedbackRepository
    {
        Task<IEnumerable<FeedbackModelData>> GetAllAsync();
        Task<FeedbackModelData> GetAsync(Guid id);
        Task<FeedbackModelData> AddAsync(FeedbackModelData ticketSales);
        Task<FeedbackModelData> DeleteAsync(Guid id);
        Task<FeedbackModelData> UpdateAsync(Guid id, FeedbackModelData blog);
    }
}
