
using Genilog_WebApi.DataContext;
using Genilog_WebApi.Model.AuthModel;

namespace Genilog_WebApi.Repository.AuthRepo
{
    public class User_RoleRepository(Genilog_Data_Context mAAP_Context) : IUser_RoleRepository
    {
        private readonly Genilog_Data_Context mAAP_Context = mAAP_Context;

        public async Task<User_Role> AddAsync(User_Role roles)
        {
            roles.Id = Guid.NewGuid();
            await mAAP_Context.User_Roles!.AddAsync(roles);
            await mAAP_Context.SaveChangesAsync();
            return roles;
        }
    }
}
