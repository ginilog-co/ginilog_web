using Genilog_WebApi.DataContext;
using Genilog_WebApi.Model.WalletModel;
using Microsoft.EntityFrameworkCore;

namespace Genilog_WebApi.Repository.WalletRepo
{
    public class WalletRepository(Genilog_Data_Context mAAP_Context) : IWalletRepository
    {
        private readonly Genilog_Data_Context mAAP_Context = mAAP_Context;

        // Payout
        public async Task<PayoutDataModel> AddPayoutAsync(PayoutDataModel bonus)
        {
            bonus.Id = Guid.NewGuid();
            await mAAP_Context.AddAsync(bonus);
            await mAAP_Context.SaveChangesAsync();
            return bonus;
        }
        public async Task<IEnumerable<PayoutDataModel>> GetAllPayoutAsync()
        {
            return await mAAP_Context.PayoutDataModels!
                .OrderBy(x => x.CreatedAt).ToListAsync();
        }

        public async Task<PayoutDataModel> GetPayoutAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await mAAP_Context.PayoutDataModels!.
               FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<PayoutDataModel> DeletePayoutAsync(Guid id)
        {
            var tickets = await mAAP_Context.PayoutDataModels!
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
                mAAP_Context.PayoutDataModels!.Remove(tickets);
                await mAAP_Context.SaveChangesAsync();
                return tickets;
            }
        }

        public async Task<PayoutDataModel> UpdatePayoutAsync(Guid id, PayoutDataModel bonus)
        {
            var dataValue = await mAAP_Context.PayoutDataModels!.FirstOrDefaultAsync(x => x.Id == id);

            if (dataValue == null)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            else
            {
                dataValue.PayOutStatus = bonus.PayOutStatus;
                dataValue.PayOutDateAt = bonus.PayOutDateAt;
                dataValue.TransactionReference= bonus.TransactionReference;
                await mAAP_Context.SaveChangesAsync();
                return dataValue;
            }
        }


    }
}

