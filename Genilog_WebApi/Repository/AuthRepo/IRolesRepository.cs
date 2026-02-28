
using Genilog_WebApi.Model.AuthModel;

namespace Genilog_WebApi.Repository.AuthRepo
{
    public interface IRolesRepository
    {
        Task<Roles> AddAsync(Roles roles);
    }
}
