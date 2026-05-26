using Microsoft.AspNetCore.Authorization;

namespace Genilog_WebApi.Repository.AuthRepo.PolicyBased
{
    public class PermissionRequirement(params string[] permissions) : IAuthorizationRequirement
    {
        public string[] Permissions { get; } = permissions;
    }

}
