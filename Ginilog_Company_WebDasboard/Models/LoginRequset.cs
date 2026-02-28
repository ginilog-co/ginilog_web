using System.ComponentModel.DataAnnotations;

namespace Ginilog_Company_WebDasboard.Models
{
    public class LoginRequset
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        public string? Email_PhoneNo { get; set; }
        public string? Password { get; set; }
    }
    public class AddDeviceToken
    {
        public string? DeviceToken { get; set; }
    }

    public class AdminLoginDto
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public string? Email { get; set; }
        public Guid UserId { get; set; }
        public string? UserType { get; set; }
        public bool? EmailVerified { get; set; }
        public bool? PhoneVerified { get; set; }
        public string? FullName { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? Branch { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

    }
}
