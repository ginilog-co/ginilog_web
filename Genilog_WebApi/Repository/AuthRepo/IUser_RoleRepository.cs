

using Genilog_WebApi.Model.AuthModel;

namespace Genilog_WebApi.Repository.AuthRepo
{
    public interface IUser_RoleRepository
    {
        Task<User_Role> AddAsync(User_Role roles);
    }
}
