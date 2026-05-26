using Genilog_WebApi.Model;
using Genilog_WebApi.Model.GeneraModel;
using Genilog_WebApi.Model.WalletModel;

namespace Genilog_WebApi.Repository.WalletRepo
{
    public interface IWalletRepository
    {
        // Transaction
        Task<TransactionDataModel> AddTransactionAsync(TransactionDataModel region);
        Task<IEnumerable<TransactionDataModel>> GetAllTransactionAsync();
        Task<PageModel<TransactionDataModelDto>> GetAllPaginatedTransactionsAsync(FilterLocationData filter);
        Task<TransactionDataModel> GetTransactionAsync(Guid id);
        Task<TransactionDataModel> DeleteTransactionAsync(Guid id);
        Task<TransactionDataModel> UpdateTransactionAsync(Guid id, TransactionDataModel region);
    }
}
