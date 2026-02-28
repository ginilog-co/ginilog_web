using Genilog_WebApi.DataContext;
using Genilog_WebApi.Model.AuthModel;


namespace Genilog_WebApi.Repository.AuthRepo
{
    public class RoleRepository(Genilog_Data_Context maap_Context) : IRolesRepository
    {
        private readonly Genilog_Data_Context maap_Context = maap_Context;

        public async Task<Roles> AddAsync(Roles roles)
        {
            roles.Id = Guid.NewGuid();
            await maap_Context.Roles!.AddAsync(roles);
            await maap_Context.SaveChangesAsync();
            return roles;
        }
    }
}
