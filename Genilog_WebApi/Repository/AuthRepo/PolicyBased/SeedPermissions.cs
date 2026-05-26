using Genilog_WebApi.Model.AuthModel;

namespace Genilog_WebApi.Repository.AuthRepo.PolicyBased
{

    public static class RolePermissionSeeder
    {
        public static async Task SeedRoles(IRolesRepository repo)
        {
            var defaultRoles = new[]
            { 
                "Super_Admin",
                "Admin",
                "Manager",
                "User",
                "Staff",
                "BrandOwner",
                "BrandStaff",
                "Rider"
            };

            foreach (var roleName in defaultRoles)
            {
                var role = await repo.GetRolesByNameAsync(roleName);
                if (role == null)
                {
                    await repo.AddRoleAsync(new Roles { Name = roleName });
                }
            }
        }

        public static async Task SeedPermissions(IRolesRepository repo)
        {
            var defaultPermissions = new[]
            {
        "CanAccessAllData",
        "CanDeleteAllData",
        "CanAssignRoles",
        // Admin Permissions
        "CanViewAdmin",
        "CanDeleteAdmin",
        "CanUpdateAdmin",
        "CanCreateAdmin",
        // User Permissions
        "CanManageUsers",
        "CanDeleteUsers",
        // Staff
        "CanCreateStaff",
        "CanManageStaff",
        "CanViewStaff",
        "CanDeleteStaff",
        //Brand
        "CanViewBrands",
        "CanManageBrands",
        "CanDeleteBrands",
        // Product
        "CanCreateProduct",
        "CanViewProduct",
        "CanManageProduct",
        "CanDeleteProduct",
        // Wallet
        "CanViewWallet",
       "CanManageWallet",

            };

            foreach (var permName in defaultPermissions)
            {
                var perm = await repo.GetPermissionByNameAsync(permName);
                if (perm == null)
                {
                    await repo.AddPermissionAsync(new Permission { Name = permName });
                }
            }
        }

        public static async Task SeedRolePermissions(IRolesRepository repo)
        {
            var superAdminRole = await repo.GetRolesByNameAsync("Super_Admin");
            if (superAdminRole == null) return;

            var permissions = await repo.GetAllPermissionsAsync();

            foreach (var permission in permissions)
            {
                var exists = await repo.RoleHasPermissionAsync(superAdminRole.Id, permission.Id);
                if (!exists)
                {
                    await repo.AddRolePermissionAsync(new RolesPermissionUsage
                    {
                        RoleId = superAdminRole.Id,
                        PermissionId = permission.Id
                    });
                }
            }
        }
    }
}
