using Microsoft.EntityFrameworkCore;
using Genilog_WebApi.DataContext;
using Genilog_WebApi.Model.Notification_Model;

namespace Genilog_WebApi.Repository.NotificationRepo
{
    public class NotificationRepository(Genilog_Data_Context mAAP_Context) : INotificationRepository
    {
        private readonly Genilog_Data_Context mAAP_Context = mAAP_Context;

        public async Task<NotificationModel> AddAsync(NotificationModel dataProvider)
        {
            dataProvider.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataProvider);
            await mAAP_Context.SaveChangesAsync();
            return dataProvider;
        }

        public async Task<NotificationModel> DeleteAsync(Guid id)
        {
            var tickets = await mAAP_Context.NotificationModels!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.NotificationModels!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<IEnumerable<NotificationModel>> GetAllAsync()
        {
            return await mAAP_Context.NotificationModels!
                  .OrderBy(x => x.CreatedAt).
                   ToListAsync();
        }

        public async Task<NotificationModel> GetAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await mAAP_Context.NotificationModels!.
               FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<NotificationModel> UpdateAsync(Guid id, NotificationModel blog)
        {
            var course = await mAAP_Context.NotificationModels!.FirstOrDefaultAsync(x => x.Id == id);
            if (course == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                course.Title = blog.Title;
                course.Body = blog.Body;
                course.IsRead = blog.IsRead;
                await mAAP_Context.SaveChangesAsync();
                return course;
            }
        }

    }
}
