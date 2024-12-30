using Genilog_WebApi.Model.AuthModel;

namespace Genilog_WebApi.Repository.AuthRepo
{
    public interface ITokenHandler
    {
        Task<string> CreateTokenAsync(GeneralUsers sub);
        Task<string> RefreshTokenAsync(string email);
    }
}
