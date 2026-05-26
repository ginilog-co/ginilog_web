using Genilog_WebApi.Model.AuthModel;

namespace Genilog_WebApi.Repository.AuthRepo.PolicyBased
{
    public static class RolePermissionHelper
    {
        // Fetch role by enum 
        public static async Task<Roles?> GetRoleAsync(RoleType roleType, IRolesRepository rolesRepository)
        {
            return await rolesRepository.GetRolesByNameAsync(roleType.ToString());
        }

        // Fetch permission by enum
        public static async Task<Permission?> GetPermissionAsync(PermissionType permissionType, IRolesRepository rolesRepository)
        {
            return await rolesRepository.GetPermissionByNameAsync(permissionType.ToString());
        }

        // Optional: assign permission to a role
        public static async Task AssignPermissionToRoleAsync(RoleType roleType, PermissionType permissionType, IRolesRepository rolesRepository)
        {
            var role = await GetRoleAsync(roleType, rolesRepository);
            var permission = await GetPermissionAsync(permissionType, rolesRepository);

            if (role != null && permission != null)
            {
                await rolesRepository.AddRolePermissionAsync(new RolesPermissionUsage
                {
                    RoleId = role.Id,
                    PermissionId = permission.Id
                });
            }
        }

        // Optional: assign permission directly to user
        public static async Task AssignPermissionToUserAsync(Guid userId, PermissionType permissionType, IRolesRepository rolesRepository)
        {
            var permission = await GetPermissionAsync(permissionType, rolesRepository);

            if (permission != null)
            {
                await rolesRepository.AddUserPermissionAsync(new UserPermissionUsage
                {
                    GeneralUsersId = userId,
                    PermissionId = permission.Id
                });
            }
        }
    }

}
