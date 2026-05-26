using AutoMapper;
using Genilog_WebApi.DataContext;
using Genilog_WebApi.Model;
using Genilog_WebApi.Model.GeneraModel;
using Genilog_WebApi.Model.WalletModel;
using Genilog_WebApi.Repository.GeneralRepo;
using Microsoft.EntityFrameworkCore;

namespace Genilog_WebApi.Repository.WalletRepo
{
    public class WalletRepository(Genilog_Data_Context mAAP_Context, IMapper mapper) : IWalletRepository
    {
        private readonly Genilog_Data_Context mAAP_Context = mAAP_Context;
        private readonly IMapper mapper = mapper;

        // Transaction
        public async Task<TransactionDataModel> AddTransactionAsync(TransactionDataModel bonus)
        {
            bonus.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(bonus);
            await mAAP_Context.SaveChangesAsync();
            return bonus;
        }
        public async Task<IEnumerable<TransactionDataModel>> GetAllTransactionAsync()
        {
            return await mAAP_Context.TransactionDataModels!
                 .AsNoTracking()
                .OrderBy(x => x.CreatedAt).ToListAsync();
        }

        public async Task<PageModel<TransactionDataModelDto>> GetAllPaginatedTransactionsAsync(FilterLocationData filter)
        {
            // 1. Base query
            var query = mAAP_Context.TransactionDataModels!
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .AsQueryable();


            // 2. Apply filters

            if (!string.IsNullOrWhiteSpace(filter.FilterTypes))
            {
                string any = filter.FilterTypes;
                query = query.Where(x =>
                    EF.Functions.Like(x.TransactionType, $"%{any}%")
                );
            }
            if (!string.IsNullOrWhiteSpace(filter.AnyItem))
            {
                string any = filter.AnyItem;
                query = query.Where(x =>
                    EF.Functions.Like(x.Title, $"%{any}%") ||
                    EF.Functions.Like(x.TrnxRef, $"%{any}%") ||
                    EF.Functions.Like(x.PhoneNo, $"%{any}%")
                );
            }
            if (!string.IsNullOrWhiteSpace(filter.UserId))
            {
                string any = filter.UserId;
                query = query.Where(x =>
                    EF.Functions.Like(x.UserId.ToString(), $"%{any}%") ||
                    EF.Functions.Like(x.AdminId.ToString(), $"%{any}%")
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
            var userDto = mapper.Map<List<TransactionDataModelDto>>(pagedData.Data);
            userDto = [.. userDto.OrderByDescending(o => o.CreatedAt)];


            // 6. Return PageModel<DTO> with same pagination metadata
            return new PageModel<TransactionDataModelDto>(
                userDto,
                pagedData.TotalCount,
                pagedData.Page,
                pagedData.PageSize
            );
        }

        public async Task<TransactionDataModel> GetTransactionAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await mAAP_Context.TransactionDataModels!
                 .AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<TransactionDataModel> DeleteTransactionAsync(Guid id)
        {
            var tickets = await mAAP_Context.TransactionDataModels!
                .FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.TransactionDataModels!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<TransactionDataModel> UpdateTransactionAsync(Guid id, TransactionDataModel bonus)
        {
            var dataValue = await mAAP_Context.TransactionDataModels!.FirstOrDefaultAsync(x => x.Id == id);

            if (dataValue == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                dataValue.TrnxStatus = bonus.TrnxStatus;
                dataValue.TrnxRef = bonus.TrnxRef;
                dataValue.Amount= bonus.Amount;
                dataValue.PhoneNo= bonus.PhoneNo;
                dataValue.Title= bonus.Title;
                dataValue.Reason= bonus.Reason;
                await mAAP_Context.SaveChangesAsync();
                return dataValue;
            }
        }

    }
}

