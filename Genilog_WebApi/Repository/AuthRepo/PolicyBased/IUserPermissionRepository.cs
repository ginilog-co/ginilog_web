namespace Genilog_WebApi.Repository.AuthRepo.PolicyBased
{
    public interface IUserPermissionRepository
    {
        Task<bool> UserHasPermissionAsync(Guid userId, string permissionName);

        Task AssignPermissionToUserAsync(Guid userId, string permissionName);

        Task AssignPermissionToRoleAsync(Guid roleId, string permissionName);

        Task<List<string>> GetUserPermissionsAsync(Guid userId);

        Task<List<string>> GetRolePermissionsAsync(Guid roleId);
        // ✅ New method to check if a permission exists by name
        Task<bool> PermissionExistsAsync(string permissionName);
    }
}
