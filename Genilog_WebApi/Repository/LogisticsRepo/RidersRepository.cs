
using AutoMapper;
using Genilog_WebApi.DataContext;
using Genilog_WebApi.Model;
using Genilog_WebApi.Model.BookingsModel;
using Genilog_WebApi.Model.GeneraModel;
using Genilog_WebApi.Model.LogisticsModel;
using Genilog_WebApi.Repository.GeneralRepo;
using Microsoft.EntityFrameworkCore;

namespace Genilog_WebApi.Repository.LogisticsRepo
{
    public class RidersRepository(Genilog_Data_Context mAAP_Context, IMapper mapper) : IRidersRepository
    {
        private readonly Genilog_Data_Context mAAP_Context = mAAP_Context;
        private readonly IMapper mapper = mapper;

        #region Riders Model Data
        public async Task<RidersModelData> AddAsync(RidersModelData dataInfo)
        {
            //dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<RidersModelData> DeleteAsync(Guid id)
        {
            var tickets = await mAAP_Context.RidersModelDatas!
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
                mAAP_Context.RidersModelDatas!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<IEnumerable<RidersModelData>> GetAllAsync()
        {
            return await mAAP_Context.RidersModelDatas!
                   .AsNoTracking()
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<PageModel<RidersModelDataDto>> GetAllPaginatedRidersAsync(FilterLocationData filter)
        {
            // 1. Base query
            var query = mAAP_Context.RidersModelDatas!
                .AsNoTracking()
                .OrderBy(x => x.CreatedAt)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.UserId))
            {
                string any = filter.UserId;
                query = query.Where(x =>
                    EF.Functions.Like(x.CompanyId.ToString(), $"%{any}%")
                );
            }

        

            if (!string.IsNullOrWhiteSpace(filter.AnyItem))
            {
                string any = filter.AnyItem;
                query = query.Where(x =>
                    EF.Functions.Like(x.AccountName, $"%{any}%") ||
                    EF.Functions.Like(x.CompanyName, $"%{any}%") ||
                    EF.Functions.Like(x.Name, $"%{any}%") ||
                    EF.Functions.Like(x.Email, $"%{any}%") ||
                    EF.Functions.Like(x.PhoneNumber, $"%{any}%") ||
                    EF.Functions.Like(x.BankName, $"%{any}%")
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
            var userDto = mapper.Map<List<RidersModelDataDto>>(pagedData.Data);
            // 6. Return PageModel<DTO> with same pagination metadata
            return new PageModel<RidersModelDataDto>(
                userDto,
                pagedData.TotalCount,
                pagedData.Page,
                pagedData.PageSize

            );
        }


        public async Task<RidersModelData> GetAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await mAAP_Context.RidersModelDatas!
                   .AsNoTracking()
                .Include(x => x.RidersReviewModels)
                .FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<RidersModelData> UpdateAsync(Guid id, RidersModelData dataInfo)
        {
            var dataValue = await mAAP_Context.RidersModelDatas!.FirstOrDefaultAsync(x => x.Id == id);

            if (dataValue == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                dataValue.Name = dataInfo.Name;
                dataValue.CompanyName = dataInfo.CompanyName;
                dataValue.PhoneNumber = dataInfo.PhoneNumber;
                dataValue.ProfilePicture = dataInfo.ProfilePicture;
                dataValue.Rating = dataValue.Rating;
                dataValue.IsVerified = dataValue.IsVerified;
                dataValue.Available = dataValue.Available;
                dataValue.Address = dataInfo.Address;
                dataValue.Locality = dataInfo.Locality;
                dataValue.State = dataInfo!.State;
                dataValue.PostCodes = dataInfo.PostCodes;
                dataValue.Latitude = dataInfo.Latitude;
                dataValue.Longitude = dataInfo.Longitude;
                dataValue.BankName = dataInfo.BankName;
                dataValue.AccountName = dataInfo.AccountName;
                dataValue.AccountNumber = dataInfo.AccountNumber;
                await mAAP_Context.SaveChangesAsync();
                return dataValue;
            }
        }

        #endregion


        #region Riders Review Data
        // Riders Review
        public async Task<RidersReviewModel> AddRidersReviewAsync(RidersReviewModel dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<IEnumerable<RidersReviewModel>> GetRidersReviewByIdAsync(Guid riderId)
        {
            return await mAAP_Context.RidersReviewModels!
                .AsNoTracking()
                .Where(x => x.RidersModelDataId == riderId)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<RidersReviewModel> DeleteRidersReviewAsync(Guid id)
        {
            var tickets = await mAAP_Context.RidersReviewModels!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.RidersReviewModels!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        #endregion

        #region Riders Chat Data
        // Chat Riders
        public async Task<IEnumerable<RidersChatModelData>> GetAllRidersChatAsync()
        {
            return await mAAP_Context.RidersChatModelDatas!.AsNoTracking().OrderBy(x => x.CreatedAt).ToListAsync();
        }

        public async Task<IEnumerable<RidersChatModelData>> GetAllRidersChatByIdAsync(Guid riderId)
        {
            return await mAAP_Context.RidersChatModelDatas!
                .AsNoTracking()
                .Where(x => x.SenderId == riderId || x.ReceiverId == riderId)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }


        public async Task<RidersChatModelData> AddRidersChatAsync(RidersChatModelData dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<RidersChatModelData> DeleteRidersChatAsync(Guid id)
        {
            var tickets = await mAAP_Context.RidersChatModelDatas!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.RidersChatModelDatas!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<RidersChatModelData> UpdateRidersChatAsync(Guid id, RidersChatModelData dataInfo)
        {
            var dataValue = await mAAP_Context.RidersChatModelDatas!.FirstOrDefaultAsync(x => x.Id == id);

            if (dataValue == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                dataValue.Message = dataInfo.Message;
                await mAAP_Context.SaveChangesAsync();
                return dataValue;
            }
        }

        public async Task<RidersChatModelData> UpdateRidersIsReadChatAsync(Guid id, RidersChatModelData dataInfo)
        {
            var dataValue = await mAAP_Context.RidersChatModelDatas!.FirstOrDefaultAsync(x => x.Id == id);

            if (dataValue == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                dataValue.IsRead = dataInfo.IsRead;
                await mAAP_Context.SaveChangesAsync();
                return dataValue;
            }
        }
        #endregion
    }
}
