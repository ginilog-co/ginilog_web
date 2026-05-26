using Genilog_WebApi.Model.GeneraModel;
using Microsoft.EntityFrameworkCore;

namespace Genilog_WebApi.Repository.GeneralRepo
{
    public static class PaginationHelper
    {
        public static async Task<PageModel<T>> ToPagedAsync<T>(this IQueryable<T> query,int? page, int? pageSize)
        {
            int totalCount = await query.CountAsync();

            // If no pagination provided → return full list
            if (!page.HasValue || !pageSize.HasValue)
            {
                var fullData = await query.ToListAsync();
                return new PageModel<T>(fullData, totalCount, 1, totalCount);
            }

            int skip = (page.Value - 1) * pageSize.Value;
            var data = await query.Skip(skip).Take(pageSize.Value).ToListAsync();
            return new PageModel<T>(data, totalCount, page.Value, pageSize.Value);
        }



    }
}
