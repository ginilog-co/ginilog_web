
using Genilog_WebApi.DataContext;
using Microsoft.EntityFrameworkCore;
using Genilog_WebApi.Model.LogisticsModel;

namespace Genilog_WebApi.Repository.LogisticsRepo
{
    public class LogisticsRepository(Genilog_Data_Context mAAP_Context) : ILogisticsRepository
    {
        private readonly Genilog_Data_Context mAAP_Context = mAAP_Context;

        public async Task<LogisticsDataModel> AddAsync(LogisticsDataModel dataInfo)
        {
            // dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<LogisticsDataModel> DeleteAsync(Guid id)
        {
            var tickets = await mAAP_Context.LogisticsDataModels!
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
                mAAP_Context.LogisticsDataModels!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<IEnumerable<LogisticsDataModel>> GetAllAsync()
        {
            return await mAAP_Context.LogisticsDataModels!
                .Include(x => x.LogisticsReviewModels)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<LogisticsDataModel> GetAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await mAAP_Context.LogisticsDataModels!
                .Include(x => x.LogisticsReviewModels)
                .FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<LogisticsDataModel> UpdateAsync(Guid id, LogisticsDataModel dataInfo)
        {
            var dataValue = await mAAP_Context.LogisticsDataModels!.FirstOrDefaultAsync(x => x.Id == id);

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
                dataValue.LogisticsLogo = dataInfo.LogisticsLogo;
                dataValue.Rating = dataValue.Rating;
                dataValue.ValueCharge = dataValue.ValueCharge;
                dataValue.CompanyAddress = dataValue.CompanyAddress;
                dataValue.CompanyInfo = dataValue.CompanyInfo;
                dataValue.NoOfTrucks = dataValue.NoOfTrucks;
                dataValue.NofOfBikes = dataValue.NofOfBikes;
                dataValue.IsVerified = dataValue.IsVerified;
                dataValue.IsIndividual = dataValue.IsIndividual;
                dataValue.Available = dataValue.Available;
                dataValue.IdCardUpload = dataValue.IdCardUpload;
                dataValue.IdCardVerification = dataValue.IdCardVerification;
                dataValue.LicenseUpload = dataValue.LicenseUpload;
                dataValue.LicenseVerification = dataValue.LicenseVerification;
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

        // Logistics Review
        public async Task<LogisticsReviewModel> AddLogisticsReviewAsync(LogisticsReviewModel dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<LogisticsReviewModel> DeleteLogisticsReviewAsync(Guid id)
        {
            var tickets = await mAAP_Context.LogisticsReviewModels!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.LogisticsReviewModels!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        // Chat Logistics
        public async Task<IEnumerable<LogisticsChatModelData>> GetAllLogisticsChatAsync()
        {
            return await mAAP_Context.LogisticsChatModelDatas!.OrderBy(x => x.CreatedAt).ToListAsync();
        }

        public async Task<LogisticsChatModelData> AddLogisticsChatAsync(LogisticsChatModelData dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
        }

        public async Task<LogisticsChatModelData> DeleteLogisticsChatAsync(Guid id)
        {
            var tickets = await mAAP_Context.LogisticsChatModelDatas!.FirstOrDefaultAsync(x => x.Id == id);
            if (tickets == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                // Delete Region
                mAAP_Context.LogisticsChatModelDatas!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<LogisticsChatModelData> UpdateLogisticsChatAsync(Guid id, LogisticsChatModelData dataInfo)
        {
            var dataValue = await mAAP_Context.LogisticsChatModelDatas!.FirstOrDefaultAsync(x => x.Id == id);

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

        public async Task<LogisticsChatModelData> UpdateLogisticsIsReadChatAsync(Guid id, LogisticsChatModelData dataInfo)
        {
            var dataValue = await mAAP_Context.LogisticsChatModelDatas!.FirstOrDefaultAsync(x => x.Id == id);

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

        // Gas Order
        public async Task<IEnumerable<OrderModelData>> GetAllOrderAsync()
        {
            return await mAAP_Context.OrderModelDatas!
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }
        public async Task<OrderModelData> GetOrderAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await mAAP_Context.OrderModelDatas
                !.FirstOrDefaultAsync(x => x.Id == id);
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
                dataValue.CurrentLatitude = dataInfo.CurrentLatitude;
                dataValue.CurrentLongitude = dataInfo.CurrentLongitude;
                dataValue.ConfirmationImage = dataInfo.ConfirmationImage;
                dataValue.ShippingCost = dataInfo.ShippingCost;
                dataValue.TrnxReference = dataInfo.TrnxReference;
                dataValue.PaymentChannel = dataInfo.PaymentChannel;
                dataValue.PaymentStatus = dataInfo.PaymentStatus;
                dataValue.Comment = dataInfo.Comment;
                dataValue.UpdatedAt = dataInfo.UpdatedAt;
                await mAAP_Context.SaveChangesAsync();
                return dataValue;
            }
        }

    

    }
}
