using Genilog_WebApi.Model.UsersDataModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Genilog_WebApi.Model.AuthModel
{
    public class GeneralUsers
    {
        public Guid Id { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? UserType { get; set; }
        public string? Email { get; set; }
        public string? PhoneNo { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public string? VerificationToken { get; set; }
        public bool? EmailConfirmed { get; set; }
        public DateTime? EmailTokenExpires { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string? PhoneVerificationToken { get; set; }
        public bool? PhoneNoConfirmed { get; set; }
        public DateTime? PhoneNoTokenExpires { get; set; }
        public DateTime? PhoneVerifiedAt { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public bool? TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockOutEnd { get; set; }
        public bool? LockOutEndEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string? ImagePath { get; set; }
        // navigation property
        [NotMapped]
        public List<string>? Roles { get; set; }
        public List<User_Role>? User_Roles { get; set; }
        public DateTime? CreatedAt { get; set; }
    }



    public class DeviceTokenModel
    {
        public Guid Id { get; set; }
        public string? DeviceTokenId { get; set; }
        public Guid UserId { get; set; }
        public string? UserType { get; set; }
    }

}
