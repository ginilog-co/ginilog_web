namespace Genilog_WebApi.Model.AuthModel
{
    public class User_Role
    {
        public Guid Id { get; set; }

        public Guid GeneralUsersId { get; set; }
        public GeneralUsers? GeneralUsers { get; set; }
        public Guid RoleId { get; set; }
        public Roles? Roles { get; set; }
    }
}
