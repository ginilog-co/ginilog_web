

using Genilog_WebApi.Model.Notification_Model;

namespace Genilog_WebApi.Repository.NotificationRepo
{
    public interface INotificationRepository
    {
        Task<IEnumerable<NotificationModel>> GetAllAsync();
        Task<NotificationModel> GetAsync(Guid id);
        Task<NotificationModel> AddAsync(NotificationModel ticketSales);
        Task<NotificationModel> DeleteAsync(Guid id);
        Task<NotificationModel> UpdateAsync(Guid id, NotificationModel blog);
    }
}
