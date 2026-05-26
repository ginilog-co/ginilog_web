using AutoMapper;
using Genilog_WebApi.DataContext;
using Genilog_WebApi.Model;
using Genilog_WebApi.Model.GeneraModel;
using Genilog_WebApi.Model.LogisticsModel;
using Genilog_WebApi.Repository.GeneralRepo;
using Microsoft.EntityFrameworkCore;

namespace Genilog_WebApi.Repository.LogisticsRepo
{
    public class CompanyRepository(Genilog_Data_Context mAAP_Context, IMapper mapper) : ICompanyRepository
    {
        private readonly Genilog_Data_Context mAAP_Context = mAAP_Context;
        private readonly IMapper mapper = mapper;

        #region Company Data
        public async Task<CompanyModelData> AddAsync(CompanyModelData dataInfo)
        {
            // dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<CompanyModelData> DeleteAsync(Guid id)
        {
            var tickets = await mAAP_Context.CompanyModelDatas!
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
                mAAP_Context.CompanyModelDatas!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<IEnumerable<CompanyModelData>> GetAllAsync()
        {
            return await mAAP_Context.CompanyModelDatas!
                   .AsNoTracking()
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<PageModel<CompanyModelDataDto>> GetAllPaginationsCompanyDataAsync(FilterLocationData filter)
        {
            // 1. Base query
            var query = mAAP_Context.CompanyModelDatas!
                .AsNoTracking()
                .OrderBy(x => x.CreatedAt)
                .AsQueryable();


            // 2. Apply filters
            if (!string.IsNullOrWhiteSpace(filter.State))
            {
                string any = filter.State;
                query = query.Where(x => EF.Functions.Like(x.State, $"%{any}%"));
            }
              
            if (!string.IsNullOrWhiteSpace(filter.Locality))
            {
                string any = filter.Locality;
                query = query.Where(x => EF.Functions.Like(x.Locality, $"%{any}%"));
            }

            if (!string.IsNullOrWhiteSpace(filter.UserId))
            {
                string any = filter.UserId;
                query = query.Where(x =>
                    EF.Functions.Like(x.AdminId.ToString(), $"%{any}%")
                );
            }

            if (!string.IsNullOrWhiteSpace(filter.FilterTypes))
            {
                string any = filter.FilterTypes.Trim().ToLower();
                query = query.Where(x =>
                    x.DeliveryTypes != null &&
                    x.DeliveryTypes.Any(d =>
                        d.Contains(any, StringComparison.CurrentCultureIgnoreCase)
                    )
                );
            }

            if (!string.IsNullOrWhiteSpace(filter.AnyItem))
            {
                string any = filter.AnyItem;
                query = query.Where(x =>
                    EF.Functions.Like(x.CompanyName, $"%{any}%") ||
                    EF.Functions.Like(x.AccountName, $"%{any}%") ||
                    EF.Functions.Like(x.AccountNumber, $"%{any}%") ||
                    EF.Functions.Like(x.BankName, $"%{any}%") ||
                    EF.Functions.Like(x.CompanyEmail, $"%{any}%") ||
                    EF.Functions.Like(x.CompanyRegNo, $"%{any}%") ||
                    EF.Functions.Like(x.PostCodes, $"%{any}%") ||
                    EF.Functions.Like(x.PhoneNumber, $"%{any}%")
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
            var userDto = mapper.Map<List<CompanyModelDataDto>>(pagedData.Data);
            userDto = [.. userDto.OrderByDescending(o => o.CreatedAt)];


            // 6. Return PageModel<DTO> with same pagination metadata
            return new PageModel<CompanyModelDataDto>(
                userDto,
                pagedData.TotalCount,
                pagedData.Page,
                pagedData.PageSize
            );
        }


        public async Task<CompanyModelData> GetAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await mAAP_Context.CompanyModelDatas!
                   .AsNoTracking()
                .Include(x => x.CompanyReviewModels)
                .FirstOrDefaultAsync(x => x.Id == id|| x.AdminId==id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<CompanyModelData> UpdateAsync(Guid id, CompanyModelData dataInfo)
        {
            var dataValue = await mAAP_Context.CompanyModelDatas!.FirstOrDefaultAsync(x => x.Id == id || x.AdminId == id);

            if (dataValue == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                dataValue.CompanyName = dataInfo.CompanyName;
                dataValue.PhoneNumber = dataInfo.PhoneNumber;
                dataValue.CompanyLogo = dataInfo.CompanyLogo;
                dataValue.Rating = dataValue.Rating;
                dataValue.ValueCharge = dataValue.ValueCharge;
                dataValue.CompanyAddress = dataValue.CompanyAddress;
                dataValue.CompanyInfo = dataValue.CompanyInfo;
                dataValue.NoOfTrucks = dataValue.NoOfTrucks;
                dataValue.NofOfBikes = dataValue.NofOfBikes;
                dataValue.CompanyRegNo = dataValue.CompanyRegNo;
                dataValue.Available = dataValue.Available;
                dataValue.Locality = dataInfo.Locality;
                dataValue.State = dataInfo!.State;
                dataValue.PostCodes = dataInfo.PostCodes;
                dataValue.Latitude = dataInfo.Latitude;
                dataValue.Longitude = dataInfo.Longitude;
                dataValue.BankName = dataInfo.BankName;
                dataValue.AccountName = dataInfo.AccountName;
                dataValue.AccountNumber = dataInfo.AccountNumber;
                dataValue.DeliveryTypes = dataInfo.DeliveryTypes;
                dataValue.ServiceAreas = dataInfo.ServiceAreas;
                await mAAP_Context.SaveChangesAsync();
                return dataValue;
            }
        }

        public async Task<bool> AdminIdExistAsync(Guid adminId)
        {
            var user = await mAAP_Context.CompanyModelDatas!.AnyAsync(x => x.AdminId == adminId);
            if (user)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion


        #region Company Review Data
        // Company Review
        public async Task<CompanyReviewModel> AddCompanyReviewAsync(CompanyReviewModel dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<IEnumerable<CompanyReviewModel>> GetCompanyReviewByIdAsync(Guid CompanyId)
        {
            return await mAAP_Context.CompanyReviewModels!
                .AsNoTracking()
                .Where(x => x.CompanyModelDataId == CompanyId)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<CompanyReviewModel> DeleteCompanyReviewAsync(Guid id)
        {
            var tickets = await mAAP_Context.CompanyReviewModels!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.CompanyReviewModels!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }
        #endregion

        #region Company Order Data
        // Order
        public async Task<IEnumerable<OrderModelData>> GetAllOrderAsync()
        {
            return await mAAP_Context.OrderModelDatas!
                  .AsNoTracking()
                 .Include(x => x.OrderDeliveryFlows)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }


        public async Task<PageModel<OrderModelDataDto>> GetAllPaginationOrderDataAsync(FilterLocationData filter)
        {
            // 1. Base query
            var query = mAAP_Context.OrderModelDatas!
                .AsNoTracking()
                  .Include(x => x.OrderDeliveryFlows)
                .OrderBy(x => x.CreatedAt)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.UserId))
            {
                string any = filter.UserId;
                query = query.Where(x =>
                    EF.Functions.Like(x.CompanyId.ToString(), $"%{any}%") ||
                    EF.Functions.Like(x.RiderId.ToString(), $"%{any}%") ||
                    EF.Functions.Like(x.StaffId.ToString(), $"%{any}%") ||
                    EF.Functions.Like(x.UserId.ToString(), $"%{any}%") ||
                    EF.Functions.Like(x.AdminId.ToString(), $"%{any}%")
                );
            }

            if (!string.IsNullOrWhiteSpace(filter.FilterTypes))
            {
                string any = filter.FilterTypes;
                query = query.Where(x =>
                    EF.Functions.Like(x.RiderType, $"%{any}%") ||
                    EF.Functions.Like(x.UserType, $"%{any}%") ||
                    EF.Functions.Like(x.OrderStatus, $"%{any}%") ||
                    EF.Functions.Like(x.PackageType, $"%{any}%") ||
                    EF.Functions.Like(x.PaymentChannel, $"%{any}%") ||
                    EF.Functions.Like(x.PurchaseChannel, $"%{any}%") ||
                    EF.Functions.Like(x.ShippingType, $"%{any}%")
                );
            }

            if (!string.IsNullOrWhiteSpace(filter.AnyItem))
            {
                string any = filter.AnyItem;
                query = query.Where(x =>
                    EF.Functions.Like(x.CompanyEmail, $"%{any}%") ||
                    EF.Functions.Like(x.CompanyName, $"%{any}%") ||
                    EF.Functions.Like(x.CompanyPhoneNo, $"%{any}%") ||
                    EF.Functions.Like(x.ItemName, $"%{any}%") ||
                    EF.Functions.Like(x.RiderName, $"%{any}%") ||

                    EF.Functions.Like(x.SenderCountry, $"%{any}%") ||
                    EF.Functions.Like(x.SenderEmail, $"%{any}%") ||
                    EF.Functions.Like(x.SenderName, $"%{any}%") ||
                    EF.Functions.Like(x.SenderPhoneNo, $"%{any}%") ||
                    EF.Functions.Like(x.SenderState, $"%{any}%") ||
                    EF.Functions.Like(x.SenderLocality, $"%{any}%") ||
                    EF.Functions.Like(x.SenderPostalCode, $"%{any}%") ||

                    EF.Functions.Like(x.RecieverCountry, $"%{any}%") ||
                    EF.Functions.Like(x.RecieverEmail, $"%{any}%") ||
                    EF.Functions.Like(x.RecieverName, $"%{any}%") ||
                    EF.Functions.Like(x.RecieverPhoneNo, $"%{any}%") ||
                    EF.Functions.Like(x.RecieverState, $"%{any}%") ||
                    EF.Functions.Like(x.RecieverLocality, $"%{any}%") ||
                    EF.Functions.Like(x.RecieverPostalCode, $"%{any}%") ||
                    EF.Functions.Like(x.TrackingNum, $"%{any}%") ||
                    EF.Functions.Like(x.TrnxReference, $"%{any}%") ||
                    EF.Functions.Like(x.StaffName, $"%{any}%")
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
            var userDto = mapper.Map<List<OrderModelDataDto>>(pagedData.Data);
            // 6. Return PageModel<DTO> with same pagination metadata
            return new PageModel<OrderModelDataDto>(
                userDto,
                pagedData.TotalCount,
                pagedData.Page,
                pagedData.PageSize

            );
        }


        public async Task<OrderModelData> GetOrderAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await mAAP_Context.OrderModelDatas!
                  .AsNoTracking()
                 .Include(x => x.OrderDeliveryFlows)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<OrderModelData?> GetOrderByTrackingNumAsync(string trackingNum)
        {
            return await mAAP_Context.OrderModelDatas!.AsNoTracking()
                 .Include(x => x.OrderDeliveryFlows)
                .FirstOrDefaultAsync(x => x.TrackingNum == trackingNum);
        }

        public async Task<OrderModelData> AddOrderAsync(OrderModelData dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<OrderModelData> DeleteOrderAsync(Guid id)
        {
            var tickets = await mAAP_Context.OrderModelDatas
                !.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
                return null;
            }
            else
            {
                // Delete Region
                mAAP_Context.OrderModelDatas!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<OrderModelData> UpdateOrderAsync(Guid id, OrderModelData dataInfo)
        {
            var dataValue = await mAAP_Context.OrderModelDatas!.FirstOrDefaultAsync(x => x.Id == id);

            if (dataValue == null)
            {
                return null;
            }
            else
            {
                dataValue.OrderStatus = dataInfo.OrderStatus;
                dataValue.ExpectedDeliveryTime = dataInfo.ExpectedDeliveryTime;
                dataValue.ConfirmationImage = dataInfo.ConfirmationImage;
                dataValue.ShippingCost = dataInfo.ShippingCost;
                dataValue.VatCost = dataInfo.VatCost;
                dataValue.TrnxReference = dataInfo.TrnxReference;
                dataValue.PaymentChannel = dataInfo.PaymentChannel;
                dataValue.PaymentStatus = dataInfo.PaymentStatus;
                dataValue.Comment = dataInfo.Comment;
                dataValue.CurrentLatitude = dataInfo.CurrentLatitude;
                dataValue.CurrentLongitude = dataInfo.CurrentLongitude;
                dataValue.CurrentLocation = dataInfo.CurrentLocation;
                dataValue.UpdatedAt = dataInfo.UpdatedAt;
                await mAAP_Context.SaveChangesAsync();
                return dataValue;
            }
        }
        public async Task<OrderModelData> AssignRiderAsync(Guid id, OrderModelData region)
        {
            var dataValue = await mAAP_Context.OrderModelDatas!.FirstOrDefaultAsync(x => x.Id == id);

            if (dataValue == null)
            {
                return null;
            }
            else
            {
                dataValue.RiderId = region.RiderId;
                dataValue.RiderName = region.RiderName;
                await mAAP_Context.SaveChangesAsync();
                return dataValue;
            }
        }
        #endregion

        #region Company Order Track Flow Data
        // order delivery flow
        public async Task<OrderDeliveryFlow> AddOrderDeliveryFlowAsync(OrderDeliveryFlow dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<OrderDeliveryFlow> DeleteOrderDeliveryFlowAsync(Guid id)
        {
            var tickets = await mAAP_Context.OrderDeliveryFlows!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.OrderDeliveryFlows!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }
        public async Task<bool> OrderDeliveryFlowExistAsync(string orderStatus, Guid orderModelId, OrderDeliveryFlow dataInfo)
        {
            var user = await mAAP_Context.OrderDeliveryFlows!.AnyAsync(x => x.OrderStatus == orderStatus && x.OrderModelDataId==orderModelId);
            if (user)
            {
                var dataValue = await mAAP_Context.OrderDeliveryFlows!.FirstOrDefaultAsync(x => x.OrderStatus == orderStatus && x.OrderModelDataId == orderModelId);
                dataValue!.OrderStatus = dataInfo.OrderStatus;
                dataValue.CurrentLatitude = dataInfo.CurrentLatitude;
                dataValue.CurrentLongitude = dataInfo.CurrentLongitude;
                dataValue.CurrentLocation = dataInfo.CurrentLocation;
                dataValue.UpdatedAt = dataInfo.UpdatedAt;
                await mAAP_Context.SaveChangesAsync();
#pragma warning disable CS8603 // Possible null reference return.
                return true;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
