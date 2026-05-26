
using Genilog_WebApi.Model.AuthModel;

namespace Genilog_WebApi.Repository.AuthRepo
{
    public interface IRolesRepository
    {
        // -----------------------------
        // ROLE CRUD
        // -----------------------------
        Task<Roles> AddRoleAsync(Roles role);
        Task<Roles?> GetRoleByIdAsync(Guid roleId);
        Task<Roles?> GetRolesByNameAsync(string roleName);
        Task<IEnumerable<Roles>> GetAllRolesAsync();
        Task<Roles> UpdateRoleAsync(Roles role);
        Task<bool> DeleteRoleAsync(Guid roleId);

        // -----------------------------
        // ASSIGN ROLE TO USER
        // -----------------------------
        Task<User_Role> AddUserRoleAsync(User_Role userRole);
        Task<bool> RemoveUserRoleAsync(Guid userId, Guid roleId);
        Task<IEnumerable<Roles>> GetUserRolesAsync(Guid userId);

        // -----------------------------
        // PERMISSION CRUD
        // -----------------------------
        Task<Permission> AddPermissionAsync(Permission permission);
        Task<IEnumerable<Permission>> GetAllPermissionsAsync();
        Task<Permission?> GetPermissionByIdAsync(Guid id);
        Task<Permission?> GetPermissionByNameAsync(string name);
        Task<Permission> UpdatePermissionAsync(Permission permission);
        Task<bool> DeletePermissionAsync(Guid permissionId);

        // -----------------------------
        // ROLE PERMISSIONS
        // -----------------------------
        Task<RolesPermissionUsage> AddRolePermissionAsync(RolesPermissionUsage rolePermission);
        Task<bool> RemoveRolePermissionAsync(Guid roleId, Guid permissionId);
        Task<IEnumerable<Permission>> GetPermissionsByRoleAsync(Guid roleId);
        Task<bool> RoleHasPermissionAsync(Guid roleId, Guid permissionId);

        // -----------------------------
        // USER PERMISSIONS (direct assignment)
        // -----------------------------
        Task<UserPermissionUsage> AddUserPermissionAsync(UserPermissionUsage userPermission);
        Task<bool> RemoveUserPermissionAsync(Guid userId, Guid permissionId);
        Task<IEnumerable<Permission>> GetUserPermissionsAsync(Guid userId);
    }
}
