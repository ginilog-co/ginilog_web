using Genilog_WebApi.DataContext;
using Genilog_WebApi.Model.AuthModel;
using Microsoft.EntityFrameworkCore;

namespace Genilog_WebApi.Repository.AuthRepo
{
    public class BlacklistedTokenRepository(Genilog_Data_Context bmg_context) : IBlacklistedTokenRepository
    {
        private readonly Genilog_Data_Context bmg_context = bmg_context;

        public async Task<BlacklistedToken> BlacklistTokenAsync(BlacklistedToken sub)
        {
            await bmg_context.AddAsync(sub);
            await bmg_context.SaveChangesAsync();
            return sub;
        }

        public async Task<bool> IsTokenBlacklistedAsync(string jti)
        {
            return await bmg_context.BlacklistedTokens!
                            .AnyAsync(t => t.Jti == jti && t.Expiry > DateTime.UtcNow);
        }

        // Optional: cleanup expired tokens
        public async Task RemoveExpiredTokensAsync()
        {
            var expired = await bmg_context.BlacklistedTokens!
                                   .Where(t => t.Expiry <= DateTime.UtcNow)
                                   .ToListAsync();
            if (expired.Count != 0)
            {
                bmg_context.BlacklistedTokens!.RemoveRange(expired);
                await bmg_context.SaveChangesAsync();
            }
        }

    }
}
