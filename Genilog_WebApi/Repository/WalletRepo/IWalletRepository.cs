using Genilog_WebApi.Model.WalletModel;

namespace Genilog_WebApi.Repository.WalletRepo
{
    public interface IWalletRepository
    {
        // Payout
        Task<PayoutDataModel> AddPayoutAsync(PayoutDataModel region);
        Task<IEnumerable<PayoutDataModel>> GetAllPayoutAsync();
        Task<PayoutDataModel> GetPayoutAsync(Guid id);
        Task<PayoutDataModel> DeletePayoutAsync(Guid id);
        Task<PayoutDataModel> UpdatePayoutAsync(Guid id, PayoutDataModel region);
    }
}
