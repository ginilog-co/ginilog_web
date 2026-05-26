using System.Data;

namespace Genilog_WebApi.Model.AuthModel
{

    public class Permission
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!; // e.g. "Orders.Create"

        public List<RolesPermissionUsage>? RolePermissions { get; set; }
        public List<UserPermissionUsage>? UserPermissions { get; set; }
    }


    public class RolesPermissionUsage
    {
        public Guid RoleId { get; set; }
        public Roles? Role { get; set; }

        public Guid PermissionId { get; set; }
        public Permission? Permission { get; set; }
    }

    public class UserPermissionUsage
    {
        public Guid GeneralUsersId { get; set; }
        public GeneralUsers? GeneralUsers { get; set; }

        public Guid PermissionId { get; set; }
        public Permission? Permission { get; set; }

    }
}
