using Genilog_WebApi.DataContext;
using Genilog_WebApi.Model.AuthModel;
using Microsoft.EntityFrameworkCore;
using System.Data;


namespace Genilog_WebApi.Repository.AuthRepo
{
    public class RoleRepository(Genilog_Data_Context context) : IRolesRepository
    {
        private readonly Genilog_Data_Context _context = context;
        // ---------------------------------------------
        // ROLE CRUD
        // ---------------------------------------------
        public async Task<Roles> AddRoleAsync(Roles role)
        {
            role.Id = Guid.NewGuid();
            await _context.Roles!.AddAsync(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<Roles?> GetRoleByIdAsync(Guid roleId)
        {
            return await _context.Roles!.AsNoTracking()
                .Include(r => r.RolePermissions)!
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == roleId);
        }
        public async Task<Roles?> GetRolesByNameAsync(string roleName)
        {
            return await _context.Roles!.AsNoTracking()
                .Include(r => r.RolePermissions)!
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Name == roleName);
        }
        public async Task<IEnumerable<Roles>> GetAllRolesAsync()
        {
            return await _context.Roles!.AsNoTracking().ToListAsync();
        }
        public async Task<Roles> UpdateRoleAsync(Roles role)
        {
            _context.Roles!.Update(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<bool> DeleteRoleAsync(Guid roleId)
        {
            var role = await _context.Roles!.FindAsync(roleId);
            if (role == null) return false;

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }

        // ---------------------------------------------
        // ASSIGN ROLE TO USER
        // ---------------------------------------------
        public async Task<User_Role> AddUserRoleAsync(User_Role userRole)
        {
            userRole.Id = Guid.NewGuid();
            await _context.User_Roles!.AddAsync(userRole);
            await _context.SaveChangesAsync();
            return userRole;
        }

        public async Task<bool> RemoveUserRoleAsync(Guid userId, Guid roleId)
        {
            var record = await _context.User_Roles!
                .FirstOrDefaultAsync(x => x.GeneralUsersId == userId && x.RoleId == roleId);

            if (record == null)
                return false;

            _context.User_Roles!.Remove(record);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Roles>> GetUserRolesAsync(Guid userId)
        {
            return await _context.User_Roles!.AsNoTracking()
                .Where(x => x.GeneralUsersId == userId)
                .Include(x => x.Roles!)
                .Select(x => x.Roles!)
                .ToListAsync();
        }

        // ---------------------------------------------
        // PERMISSION CRUD
        // ---------------------------------------------

        public async Task<Permission> AddPermissionAsync(Permission permission)
        {
            permission.Id = Guid.NewGuid();
            await _context.Permissions!.AddAsync(permission);
            await _context.SaveChangesAsync();
            return permission;
        }

        public async Task<IEnumerable<Permission>> GetAllPermissionsAsync()
        {
            return await _context.Permissions!.AsNoTracking().ToListAsync();
        }

        public async Task<Permission?> GetPermissionByIdAsync(Guid id)
        {
            return await _context.Permissions!.FindAsync(id);
        }
        public async Task<Permission?> GetPermissionByNameAsync(string name)
        {
            return await _context.Permissions!.AsNoTracking()
                .FirstOrDefaultAsync(p => p.Name == name);
        }


        public async Task<Permission> UpdatePermissionAsync(Permission permission)
        {
            _context.Permissions!.Update(permission);
            await _context.SaveChangesAsync();
            return permission;
        }

        public async Task<bool> DeletePermissionAsync(Guid permissionId)
        {
            var permission = await _context.Permissions!.FindAsync(permissionId);
            if (permission == null)
                return false;

            _context.Permissions.Remove(permission);
            await _context.SaveChangesAsync();
            return true;
        }

        // ---------------------------------------------
        // ROLE PERMISSIONS
        // ---------------------------------------------
        public async Task<RolesPermissionUsage> AddRolePermissionAsync(RolesPermissionUsage rolePermission)
        {
            await _context.RolesPermissionUsages!.AddAsync(rolePermission);
            await _context.SaveChangesAsync();
            return rolePermission;
        }

        public async Task<bool> RemoveRolePermissionAsync(Guid roleId, Guid permissionId)
        {
            var record = await _context.RolesPermissionUsages!
                .FirstOrDefaultAsync(x => x.RoleId == roleId && x.PermissionId == permissionId);

            if (record == null)
                return false;

            _context.RolesPermissionUsages!.Remove(record);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Permission>> GetPermissionsByRoleAsync(Guid roleId)
        {
            return await _context.RolesPermissionUsages!.AsNoTracking()
                .Where(x => x.RoleId == roleId)
                .Include(x => x.Permission!)
                .Select(x => x.Permission!)
                .ToListAsync();
        }

        public async Task<bool> RoleHasPermissionAsync(Guid roleId, Guid permissionId)
        {
            return await _context.RolesPermissionUsages!
                .AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
        }


        // ---------------------------------------------
        // USER PERMISSIONS (direct assignment)
        // ---------------------------------------------
        public async Task<UserPermissionUsage> AddUserPermissionAsync(UserPermissionUsage userPermission)
        {
            await _context.UserPermissionUsages!.AddAsync(userPermission);
            await _context.SaveChangesAsync();
            return userPermission;
        }

        public async Task<bool> RemoveUserPermissionAsync(Guid userId, Guid permissionId)
        {
            var record = await _context.UserPermissionUsages!
                .FirstOrDefaultAsync(x => x.GeneralUsersId == userId && x.PermissionId == permissionId);

            if (record == null)
                return false;

            _context.UserPermissionUsages!.Remove(record);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Permission>> GetUserPermissionsAsync(Guid userId)
        {
            return await _context.UserPermissionUsages!.AsNoTracking()
                .Where(x => x.GeneralUsersId == userId)
                .Include(x => x.Permission!)
                .Select(x => x.Permission!)
                .ToListAsync();
        }
    }
}
