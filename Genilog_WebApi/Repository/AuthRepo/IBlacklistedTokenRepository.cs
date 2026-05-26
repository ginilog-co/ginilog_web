using Genilog_WebApi.Model.AuthModel;

namespace Genilog_WebApi.Repository.AuthRepo
{
    public interface IBlacklistedTokenRepository
    {
        Task<BlacklistedToken> BlacklistTokenAsync(BlacklistedToken sub);
        Task<bool> IsTokenBlacklistedAsync(string jti);

        Task RemoveExpiredTokensAsync();
    }
}
