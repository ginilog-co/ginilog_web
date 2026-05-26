using AutoMapper;
using Genilog_WebApi.DataContext;
using Genilog_WebApi.Model;
using Genilog_WebApi.Model.BookingsModel;
using Genilog_WebApi.Model.GeneraModel;
using Genilog_WebApi.Repository.GeneralRepo;
using Microsoft.EntityFrameworkCore;

namespace Genilog_WebApi.Repository.BookingsRepo
{
    public class AirlineRepository(Genilog_Data_Context mAAP_Context, IMapper mapper) : IAirlineRepository
    {
        private readonly Genilog_Data_Context mAAP_Context = mAAP_Context;
        private readonly IMapper mapper = mapper;

        #region Airline Data
        public async Task<AirlineDataModel> AddAsync(AirlineDataModel dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<AirlineDataModel> DeleteAsync(Guid id)
        {
            var tickets = await mAAP_Context.AirlineDataModels!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.AirlineDataModels!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<IEnumerable<AirlineDataModel>> GetAllAsync()
        {
            return await mAAP_Context.AirlineDataModels!
                  .AsNoTracking()
                  .Include(x => x.AirlineImages)
                  .OrderBy(x => x.CreatedAt).
                   ToListAsync();
        }

        public async Task<PageModel<AirlineDataModelDto>> GetAllPaginationsAirlineAsync(FilterLocationData filter)
        {
            // 1. Base query
            var query = mAAP_Context.AirlineDataModels!
                .AsNoTracking()
                .Include(x => x.AirlineImages)
                .OrderBy(x => x.CreatedAt)
                .AsQueryable();


            // 2. Apply filters
            if (!string.IsNullOrWhiteSpace(filter.State))
                query = query.Where(x => x.State!.Contains(filter.State));

            if (!string.IsNullOrWhiteSpace(filter.Locality))
                query = query.Where(x => x.Locality!.Contains(filter.Locality));

            if (!string.IsNullOrWhiteSpace(filter.UserId))
            {
                string any = filter.UserId;
                query = query.Where(x =>
                    EF.Functions.Like(x.AdminId.ToString(), $"%{any}%")
                );
            }

            if (!string.IsNullOrWhiteSpace(filter.FilterTypes))
            {
                string any = filter.FilterTypes;
                query = query.Where(x =>
                    EF.Functions.Like(x.AirlineType, $"%{any}%")
                );
            }

            if (!string.IsNullOrWhiteSpace(filter.AnyItem))
            {
                string any = filter.AnyItem;
                query = query.Where(x =>
                    EF.Functions.Like(x.AirlineName, $"%{any}%") ||
                    EF.Functions.Like(x.AirlinePhoneNo, $"%{any}%") ||
                    EF.Functions.Like(x.AirlineEmail, $"%{any}%") ||
                    EF.Functions.Like(x.AirlinePhoneNo, $"%{any}%") ||
                    EF.Functions.Like(x.AirlineWebsite, $"%{any}%") 
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
            var userDto = mapper.Map<List<AirlineDataModelDto>>(pagedData.Data);
            userDto = [.. userDto.OrderByDescending(o => o.CreatedAt)];


            // 6. Return PageModel<DTO> with same pagination metadata
            return new PageModel<AirlineDataModelDto>(
                userDto,
                pagedData.TotalCount,
                pagedData.Page,
                pagedData.PageSize
            );
        }

        public async Task<AirlineDataModel> GetAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await mAAP_Context.AirlineDataModels!.AsNoTracking()
                  .Include(x => x.AirlineImages)
                  .Include(x => x.AirCraftList)
                   .Include(x => x.AirLineServiceLocations)
                  .Include(x => x.AirlineReviewModels).
               FirstOrDefaultAsync(x => x.Id == id || x.AdminId==id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<AirlineDataModel> UpdateAsync(Guid id, AirlineDataModel dataInfo)
        {
            var dataValue = await mAAP_Context.AirlineDataModels!.FirstOrDefaultAsync(x => x.Id == id || x.AdminId==id);

            if (dataValue == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                dataValue.AirlineName = dataInfo.AirlineName;
                dataValue.AirlineEmail = dataInfo.AirlineEmail;
                dataValue.AirlineInfo = dataInfo.AirlineInfo;
                dataValue.AirlineType = dataInfo.AirlineType;
                dataValue.AirlineWebsite = dataInfo.AirlineWebsite;
                dataValue.AirlinePhoneNo = dataInfo.AirlinePhoneNo;
                dataValue.State = dataInfo.State;
                dataValue.Locality = dataInfo.Locality;
                dataValue.Country = dataInfo.Country;
                dataValue.Rating = dataValue.Rating;
                dataValue.BookingAmount = dataInfo.BookingAmount;
                dataValue.Available = dataInfo.Available;
                dataValue.AirlineLogo = dataInfo.AirlineLogo;
                await mAAP_Context.SaveChangesAsync();
                return dataValue;
            }
        }
        public async Task<bool> AdminIdExistAsync(Guid adminId)
        {
            var dataValue = await mAAP_Context.AirlineDataModels!.FirstOrDefaultAsync(x => x.AdminId == adminId);
            if (dataValue == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion

        public async Task<AirCraftList> AddAirCraftListAsync(AirCraftList dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<AirCraftList> DeleteAirCraftListAsync(Guid id)
        {
            var tickets = await mAAP_Context.AirCraftList!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.AirCraftList!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<AirlineReviewModel> AddAirlineReviewAsync(AirlineReviewModel dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<AirlineReviewModel> DeleteAirlineReviewAsync(Guid id)
        {
            var tickets = await mAAP_Context.AirlineReviewModels!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.AirlineReviewModels!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<AirLineServiceLocation> AddAirLineServiceLocationAsync(AirLineServiceLocation dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<AirLineServiceLocation> DeleteAirLineServiceLocationAsync(Guid id)
        {
            var tickets = await mAAP_Context.AirLineServiceLocations!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.AirLineServiceLocations!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        // Airline Ticket
        public async Task<IEnumerable<FlightTicketBookModel>> GetAllAirlineFlightTicketAsync()
        {
            return await mAAP_Context.FlightTicketBookModels!.OrderBy(x => x.CreatedAt).ToListAsync();
        }

        public async Task<FlightTicketBookModel> AddAirlineFlightTicketAsync(FlightTicketBookModel dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<FlightTicketBookModel> GetAirlineFlightTicketAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await mAAP_Context.FlightTicketBookModels!.
               FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<FlightTicketBookModel> DeleteAirlineFlightTicketAsync(Guid id)
        {
            var tickets = await mAAP_Context.FlightTicketBookModels!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.FlightTicketBookModels!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<FlightTicketBookModel> UpdateAirlineFlightTicketAsync(Guid id, FlightTicketBookModel dataInfo)
        {
            var dataValue = await mAAP_Context.FlightTicketBookModels!.FirstOrDefaultAsync(x => x.Id == id);

            if (dataValue == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                dataValue.DapatureTime = dataInfo.DapatureTime;
                dataValue.AvailabeTimeInterval = dataInfo.AvailabeTimeInterval;
                dataValue.Dapature = dataInfo.Dapature;
                dataValue.Destination = dataInfo.Destination;
                dataValue.Available = dataInfo.Available;
                dataValue.IsReturn = dataInfo.IsReturn;
                dataValue.TicketPrice = dataInfo.TicketPrice;
                await mAAP_Context.SaveChangesAsync();
                return dataValue;
            }
        }

        // Places Chat
        public async Task<IEnumerable<AirlineChatModel>> GetAllAirlineChatAsync()
        {
            return await mAAP_Context.AirlineChatModels!.OrderBy(x => x.CreatedAt).ToListAsync();
        }

        public async Task<AirlineChatModel> AddAirlineChatAsync(AirlineChatModel dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<AirlineChatModel> DeleteAirlineChatAsync(Guid id)
        {
            var tickets = await mAAP_Context.AirlineChatModels!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.AirlineChatModels!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<AirlineChatModel> UpdateAirlineChatAsync(Guid id, AirlineChatModel dataInfo)
        {
            var dataValue = await mAAP_Context.AirlineChatModels!.FirstOrDefaultAsync(x => x.Id == id);

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

        public async Task<AirlineChatModel> UpdateAirlinesIsReadChatAsync(Guid id, AirlineChatModel dataInfo)
        {
            var dataValue = await mAAP_Context.AirlineChatModels!.FirstOrDefaultAsync(x => x.Id == id);

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
    }
}
