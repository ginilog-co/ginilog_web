using Genilog_WebApi.DataContext;
using Genilog_WebApi.Model.InfoModel;
using Microsoft.EntityFrameworkCore;

namespace Genilog_WebApi.Repository.InfoRepo
{
    public class FeedbackRepository(Genilog_Data_Context mAAP_Context) : IFeedbackRepository
    {
        private readonly Genilog_Data_Context mAAP_Context = mAAP_Context;

        public async Task<FeedbackModelData> AddAsync(FeedbackModelData dataProvider)
        {
            dataProvider.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataProvider);
            await mAAP_Context.SaveChangesAsync();
            return dataProvider;
        }

        public async Task<FeedbackModelData> DeleteAsync(Guid id)
        {

            var tickets = await mAAP_Context.FeedbackModelDatas!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.FeedbackModelDatas!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<IEnumerable<FeedbackModelData>> GetAllAsync()
        {
            return await mAAP_Context.FeedbackModelDatas!
                  .OrderBy(x => x.CreatedAt).
                   ToListAsync();
        }

        public async Task<FeedbackModelData> GetAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await mAAP_Context.FeedbackModelDatas!.
               FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<FeedbackModelData> UpdateAsync(Guid id, FeedbackModelData blog)
        {
            var course = await mAAP_Context.FeedbackModelDatas!.FirstOrDefaultAsync(x => x.Id == id);
            if (course == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                course.Feedback = blog.Feedback;
                await mAAP_Context.SaveChangesAsync();
                return course;
            }
        }
    }
}
