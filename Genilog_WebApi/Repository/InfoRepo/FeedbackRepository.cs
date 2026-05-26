using AutoMapper;
using Genilog_WebApi.DataContext;
using Genilog_WebApi.Model;
using Genilog_WebApi.Model.GeneraModel;
using Genilog_WebApi.Model.InfoModel;
using Genilog_WebApi.Repository.GeneralRepo;
using Microsoft.EntityFrameworkCore;

namespace Genilog_WebApi.Repository.InfoRepo
{
    public class FeedbackRepository(Genilog_Data_Context mAAP_Context, IMapper mapper) : IFeedbackRepository
    {
        private readonly Genilog_Data_Context mAAP_Context = mAAP_Context;
        private readonly IMapper mapper = mapper;

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
                   .AsNoTracking()
                  .OrderBy(x => x.CreatedAt).
                   ToListAsync();
        }

        public async Task<PageModel<FeedbackModelDataDto>> GetAllPaginatedAsync(FilterLocationData filter)
        {
            // 1. Base query
            var query = mAAP_Context.FeedbackModelDatas!
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .AsQueryable();


            // 2. Apply filters

       
            if (!string.IsNullOrWhiteSpace(filter.AnyItem))
            {
                string any = filter.AnyItem;
                query = query.Where(x =>
                    EF.Functions.Like(x.Name, $"%{any}%") ||
                    EF.Functions.Like(x.CompanyName, $"%{any}%") ||
                    EF.Functions.Like(x.Email, $"%{any}%") ||
                    EF.Functions.Like(x.PhoneNo, $"%{any}%")
                );
            }

            if (filter.StartDate.HasValue && filter.EndDate.HasValue)
            {
                var from = filter.StartDate.Value.Date;
                var to = filter.EndDate.Value.Date;

                // swap if reversed
                if (from > to)
                    (from, to) = (to, from);



                var fromDate = from;
                var toDate = to.AddDays(1);

                query = query.Where(s => s.CreatedAt >= fromDate && s.CreatedAt < toDate);

                // query = query.Where(s => s.CreatedAt.Date >= from && s.CreatedAt.Date <= to);
            }

            // 3. Pagination on the entity query
            var pageSize = Math.Clamp(filter.PageSize ?? 20, 1, 20);
            var page = filter.Page is null or <= 0 ? 1 : filter.Page.Value;
            var pagedData = await query.ToPagedAsync(page, pageSize);


            // 4. Map to DTO after retrieving paged data
            var userDto = mapper.Map<List<FeedbackModelDataDto>>(pagedData.Data);
            userDto = [.. userDto.OrderByDescending(o => o.CreatedAt)];


            // 6. Return PageModel<DTO> with same pagination metadata
            return new PageModel<FeedbackModelDataDto>(
                userDto,
                pagedData.TotalCount,
                pagedData.Page,
                pagedData.PageSize
            );
        }

        public async Task<FeedbackModelData> GetAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await mAAP_Context.FeedbackModelDatas!
                   .AsNoTracking().
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
