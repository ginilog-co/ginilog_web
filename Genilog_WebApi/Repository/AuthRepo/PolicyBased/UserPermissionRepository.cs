using Genilog_WebApi.DataContext;
using Genilog_WebApi.Model.AuthModel;
using Microsoft.EntityFrameworkCore;
using System;

namespace Genilog_WebApi.Repository.AuthRepo.PolicyBased
{
    public class UserPermissionRepository(Genilog_Data_Context context) : IUserPermissionRepository
    {
        private readonly Genilog_Data_Context _context = context;

        public async Task<bool> UserHasPermissionAsync(Guid userId, string permissionName)
        {
            return await _context.UserPermissionUsages!
                .Include(x => x.Permission)
                .AnyAsync(x => x.GeneralUsersId == userId && x.Permission!.Name == permissionName);
        }

        public async Task AssignPermissionToUserAsync(Guid userId, string permissionName)
        {
            var permission = await _context.Permissions!.FirstOrDefaultAsync(x => x.Name == permissionName);

            if (permission == null)
            {
                permission = new Permission { Name = permissionName };
                _context.Permissions!.Add(permission);
                await _context.SaveChangesAsync();
            }

            var exists = await _context.UserPermissionUsages!.AnyAsync(x => x.GeneralUsersId == userId && x.PermissionId == permission.Id);

            if (!exists)
            {
                _context.UserPermissionUsages!.Add(new UserPermissionUsage
                {
                    GeneralUsersId = userId,
                    PermissionId = permission.Id
                });

                await _context.SaveChangesAsync();
            }
        }

        public async Task AssignPermissionToRoleAsync(Guid roleId, string permissionName)
        {
            var permission = await _context.Permissions!.FirstOrDefaultAsync(x => x.Name == permissionName);

            if (permission == null)
            {
                permission = new Permission { Name = permissionName };
                _context.Permissions!.Add(permission);
                await _context.SaveChangesAsync();
            }

            var exists = await _context.RolesPermissionUsages!
                .AnyAsync(x => x.RoleId == roleId && x.PermissionId == permission.Id);

            if (!exists)
            {
                _context.RolesPermissionUsages!.Add(new RolesPermissionUsage
                {
                    RoleId = roleId,
                    PermissionId = permission.Id
                });

                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<string>> GetUserPermissionsAsync(Guid userId)
        {
            return await _context.UserPermissionUsages!.AsNoTracking()
                .Where(x => x.GeneralUsersId == userId)
                .Select(x => x.Permission!.Name)
                .ToListAsync();
        }

        public async Task<List<string>> GetRolePermissionsAsync(Guid roleId)
        {
            return await _context.RolesPermissionUsages!.AsNoTracking()
                .Where(x => x.RoleId == roleId)
                .Select(x => x.Permission!.Name)
                .ToListAsync();
        }

        public async Task<bool> PermissionExistsAsync(string permissionName)
        {
            return await _context.Permissions!
                .AnyAsync(p => p.Name == permissionName);
        }
    }
}
