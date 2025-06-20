
using Genilog_WebApi.Model.AuthModel;
using Genilog_WebApi.Model.UsersDataModel;

namespace Genilog_WebApi.Repository.AuthRepo
{
    public interface IGeneralUserRepository
    {
        Task<IEnumerable<GeneralUsers>> GetAllAsync();
        Task<GeneralUsers> GetAsync(Guid id);
        Task<GeneralUsers> AddAsync(GeneralUsers region, string password);
        Task<GeneralUsers> DeleteAsync(Guid id);
        Task<GeneralUsers> UpdateAsync(Guid id, GeneralUsers region);
        Task<bool> UserExistAsync(string email);
        Task<bool> UserPhoneNoExistAsync(string phoneNo);
        Task<bool> UserNameExistAsync(string userName);
        Task<GeneralUsers> AuthenticateAsync(string email, string password);
        Task<GeneralUsers> VerifyAsync(string token);
        Task<GeneralUsers> PhoneNoVerifyAsync(string otp);
        Task<GeneralUsers> ForgetPasswordAsync(string email);
        Task<GeneralUsers> RequestNewEmailTokenAsync(string email);
        Task<GeneralUsers> RequestNewPhoneNoTokenAsync(string phoneNo);
        Task<GeneralUsers> PasswordResetAsync(string token, string password);
        Task<GeneralUsers> TwoFactorEnabledAsync(Guid id);
        Task<GeneralUsers> CheckUserAsync(Guid id);

        // Device Token
        Task<IEnumerable<DeviceTokenModel>> GetAllDeviceTokenAsync();
        Task<DeviceTokenModel> AddDeviceTokenModelAsync(DeviceTokenModel region);
        Task<DeviceTokenModel> DeleteDeviceTokenModelAsync(string id);
        Task<DeviceTokenModel> UpdateDeviceTokenModelAsync(Guid id, DeviceTokenModel region);
        Task<bool> DeviceTokenExistAsync(string token);

    }
}
