
using Genilog_WebApi.DataContext;
using Microsoft.EntityFrameworkCore;
using Genilog_WebApi.Model.LogisticsModel;

namespace Genilog_WebApi.Repository.LogisticsRepo
{
    public class RidersRepository(Genilog_Data_Context mAAP_Context) : IRidersRepository
    {
        private readonly Genilog_Data_Context mAAP_Context = mAAP_Context;

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
                .Include(x => x.RidersReviewModels)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<RidersModelData> GetAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await mAAP_Context.RidersModelDatas!
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

        // Riders Review
        public async Task<RidersReviewModel> AddRidersReviewAsync(RidersReviewModel dataInfo)
        {
            dataInfo.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(dataInfo);
            await mAAP_Context.SaveChangesAsync();
            return dataInfo;
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

        // Chat Riders
        public async Task<IEnumerable<RidersChatModelData>> GetAllRidersChatAsync()
        {
            return await mAAP_Context.RidersChatModelDatas!.OrderBy(x => x.CreatedAt).ToListAsync();
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
