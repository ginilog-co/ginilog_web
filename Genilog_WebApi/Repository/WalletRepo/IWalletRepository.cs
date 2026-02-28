using Genilog_WebApi.Model.WalletModel;

namespace Genilog_WebApi.Repository.WalletRepo
{
    public interface IWalletRepository
    {
        // Transaction
        Task<TransactionDataModel> AddTransactionAsync(TransactionDataModel region);
        Task<IEnumerable<TransactionDataModel>> GetAllTransactionAsync();
        Task<TransactionDataModel> GetTransactionAsync(Guid id);
        Task<TransactionDataModel> DeleteTransactionAsync(Guid id);
        Task<TransactionDataModel> UpdateTransactionAsync(Guid id, TransactionDataModel region);
    }
}
