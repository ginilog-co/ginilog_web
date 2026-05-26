using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace Genilog_WebApi.Repository.AuthRepo.PolicyBased
{
    public class PermissionAuthorizationHandler(IUserPermissionRepository permissionRepo)
                : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IUserPermissionRepository _permissionRepo = permissionRepo;

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return;

            var userId = Guid.Parse(userIdClaim.Value);

            // requirement.Permissions → array of permissions
            var userPermissions = await _permissionRepo
                .GetUserPermissionsAsync(userId);

            // ANY-of
            if (userPermissions.Any(p => requirement.Permissions.Contains(p)))
            {
                context.Succeed(requirement);
            }
        }
    }


}
