namespace Genilog_WebApi.Model.AuthModel
{
    public class Roles
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public List<User_Role>? User_Roles { get; set; }

    }
}
