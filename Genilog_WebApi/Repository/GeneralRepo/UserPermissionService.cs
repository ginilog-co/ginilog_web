

using Genilog_WebApi.Model.AuthModel;
using Genilog_WebApi.Repository.AuthRepo;
using Genilog_WebApi.Repository.AuthRepo.PolicyBased;

namespace Genilog_WebApi.Repository.GeneralRepo
{
    public class UserPermissionService(IRolesRepository rolesRepository) : IUserPermissionService
    {
        private readonly IRolesRepository _rolesRepository = rolesRepository;

        public async Task AddPermissionToUserAsync(
            Guid userId,
            PermissionType permissionToAdd)
        {
            // 1️⃣ Resolve permission
            var permission = await RolePermissionHelper
                .GetPermissionAsync(permissionToAdd, _rolesRepository);

            if (permission == null)
                return; // permission does not exist → nothing to add

            // 2️⃣ Check existing permissions
            var userPermissions = await _rolesRepository
                .GetUserPermissionsAsync(userId);

            if (userPermissions.Any(p => p.Id == permission.Id))
                return; // already exists → do nothing

            // 3️⃣ Add permission
            await _rolesRepository.AddUserPermissionAsync(new UserPermissionUsage
            {
                GeneralUsersId = userId,
                PermissionId = permission.Id
            });
        }


        public async Task RemovePermissionFromUserAsync(
       Guid userId,
       PermissionType permissionToRemove)
        {
            var permission = await RolePermissionHelper
                .GetPermissionAsync(permissionToRemove, _rolesRepository);

            if (permission == null)
                return; // permission does not exist → nothing to remove

            var userPermissions = await _rolesRepository
                .GetUserPermissionsAsync(userId);

            var userPermissionEntry = userPermissions
                .FirstOrDefault(p => p.Id == permission.Id);

            if (userPermissionEntry == null)
                return; // user doesn't have it → nothing to remove

            await _rolesRepository
                .RemoveUserPermissionAsync(userId, permission.Id);
        }
    }
 }
