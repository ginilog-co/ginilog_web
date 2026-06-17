using Genilog_WebApi.Model.UsersDataModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Genilog_WebApi.Model.AuthModel
{

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RoleType
    {
        User,
        Super_Admin,
        Admin,
        Staff,
        Manager,
        BrandOwner,
        BrandStaff,
        Rider
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PermissionType
    {
        CanAccessAllData,
        CanDeleteAllData,
        CanAssignRoles,
        // Admin Permissions
        CanViewAdmin,
        CanDeleteAdmin,
        CanUpdateAdmin,
        CanCreateAdmin,
        // User Permissions
        CanManageUsers,
        CanDeleteUsers,
        // Staff
        CanCreateStaff,
        CanManageStaff,
        CanViewStaff,
        CanDeleteStaff,
        //Brand
        CanViewBrands,
        CanManageBrands,
        CanDeleteBrands,
        // Product
        CanCreateProduct,
        CanViewProduct,
        CanManageProduct,
        CanDeleteProduct,
        // Wallet
        CanViewWallet,
        CanManageWallet,
    }

    public class GeneralUsers
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
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
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public bool? TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockOutEnd { get; set; }
        public bool? LockOutEndEnabled { get; set; }
        public int AccessFailedCount { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string? ImagePath { get; set; }
        public List<User_Role>? User_Roles { get; set; }
        public List<UserPermissionUsage>? UserPermissions { get; set; }

        // Correct navigation properties
        [NotMapped]
        public List<string> Roles => User_Roles?.Select(x => x.Roles?.Name ?? "").ToList() ?? [];
        [NotMapped]
        public List<string> Permissions => UserPermissions?.Select(x => x.Permission?.Name ?? "").ToList() ?? [];

        public DateTime? CreatedAt { get; set; }
        public bool ActivateWallet { get; set; }
        public bool SuspendedAccount { get; set; }
        public bool ArchivedAccount { get; set; }
        public string? TwoFactorSecret { get; set; }

        public string? TwoFactorRecoveryCodes { get; set; }
        public string? TwoFactorRecoveryCodesHash { get; set; }
    }

    public class DeviceTokenModel
    {
        public Guid Id { get; set; }
        public string? DeviceTokenId { get; set; }
        public Guid UserId { get; set; }
        public string? UserType { get; set; }
    }

    public class TwoFactorCodeModel
    {
        public string? Secret { get; set; }
        public string? QrCodeUrl { get; set; }
        public string? Message { get; set; }
        public string[]? RawCodes { get; set; }
    }

    public class VerifyTwoFactorRequest
    {
        public string Code { get; set; } = string.Empty;
    }

}
