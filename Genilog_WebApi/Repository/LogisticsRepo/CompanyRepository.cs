using Genilog_WebApi.DataContext;
using Genilog_WebApi.Model.LogisticsModel;
using Microsoft.EntityFrameworkCore;

namespace Genilog_WebApi.Repository.LogisticsRepo
{
    public class CompanyRepository(Genilog_Data_Context mAAP_Context) : ICompanyRepository
    {
        private readonly Genilog_Data_Context mAAP_Context = mAAP_Context;

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
                .Include(x => x.CompanyReviewModels)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<CompanyModelData> GetAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await mAAP_Context.CompanyModelDatas!
                .Include(x => x.CompanyReviewModels)
                .FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<CompanyModelData> UpdateAsync(Guid id, CompanyModelData dataInfo)
        {
            var dataValue = await mAAP_Context.CompanyModelDatas!.FirstOrDefaultAsync(x => x.Id == id);

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
                await mAAP_Context.SaveChangesAsync();
                return dataValue;
            }
        }

        // Company Review
        public async Task<CompanyReviewModel> AddCompanyReviewAsync(CompanyReviewModel dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
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

        // Order
        public async Task<IEnumerable<OrderModelData>> GetAllOrderAsync()
        {
            return await mAAP_Context.OrderModelDatas!
                 .Include(x => x.PackageImageLists)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }
        public async Task<OrderModelData> GetOrderAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await mAAP_Context.OrderModelDatas!
                 .Include(x => x.PackageImageLists)
                .FirstOrDefaultAsync(x => x.Id == id);
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
        // Order Images
        public async Task<PackageImageList> AddPackageImagesAsync(PackageImageList region)
        {
            region.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(region);
            await mAAP_Context.SaveChangesAsync();
            return region;
        }

        public async Task<PackageImageList> DeletePackageImagesAsync(Guid id)
        {
            var tickets = await mAAP_Context.PackageImageLists!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
            }
            else
            {
                // Delete Region
                mAAP_Context.PackageImageLists!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }
    }
}
