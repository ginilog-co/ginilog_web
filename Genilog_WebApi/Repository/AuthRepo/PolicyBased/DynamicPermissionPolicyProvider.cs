using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Genilog_WebApi.Repository.AuthRepo.PolicyBased
{
    public class DynamicPermissionPolicyProvider(
        IOptions<AuthorizationOptions> options,
        IServiceProvider serviceProvider) : IAuthorizationPolicyProvider
    {
        private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider = new(options);
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallbackPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallbackPolicyProvider.GetFallbackPolicyAsync();

        public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            // 1️⃣ First try to get the policy normally (for manually created policies)
            var existing = await _fallbackPolicyProvider.GetPolicyAsync(policyName);
            if (existing != null)
                return existing;

            // 2️⃣ For dynamic permissions: check if this is a real permission (single permission)
            using var scope = _serviceProvider.CreateScope();
            var permissionRepo = scope.ServiceProvider.GetRequiredService<IUserPermissionRepository>();

            bool existsAsPermission = await permissionRepo.PermissionExistsAsync(policyName);

            if (!existsAsPermission)
                return null;

            // 3️⃣ Create dynamic policy for that SINGLE permission
            return new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(policyName))
                .Build();
        }
    }


}
