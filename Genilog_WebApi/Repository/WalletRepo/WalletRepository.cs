using Genilog_WebApi.DataContext;
using Genilog_WebApi.Model.WalletModel;
using Microsoft.EntityFrameworkCore;

namespace Genilog_WebApi.Repository.WalletRepo
{
    public class WalletRepository(Genilog_Data_Context mAAP_Context) : IWalletRepository
    {
        private readonly Genilog_Data_Context mAAP_Context = mAAP_Context;

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
                .OrderBy(x => x.CreatedAt).ToListAsync();
        }

        public async Task<TransactionDataModel> GetTransactionAsync(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await mAAP_Context.TransactionDataModels!.
               FirstOrDefaultAsync(x => x.Id == id);
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

