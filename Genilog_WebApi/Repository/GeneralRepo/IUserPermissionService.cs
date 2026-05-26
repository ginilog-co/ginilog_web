using Genilog_WebApi.Model.AuthModel;

namespace Genilog_WebApi.Repository.GeneralRepo
{
    public interface IUserPermissionService
    {
        Task AddPermissionToUserAsync(Guid userId, PermissionType permissionToAdd);
        Task RemovePermissionFromUserAsync(Guid userId, PermissionType permissionToRemove);
    }
}
