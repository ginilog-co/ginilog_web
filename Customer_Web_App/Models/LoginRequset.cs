using System.ComponentModel.DataAnnotations;

namespace Customer_Web_App.Models
{
    public class LoginRequset
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        public string? Email_PhoneNo { get; set; }
        public string? Password { get; set; }

        public string? IdToken { get; set; }
    }
    public class AddDeviceToken
    {
        public string? DeviceToken { get; set; }
    }

    public class LoginExternalRequset
    {
        public string? Email { get; set; }
        public string? IdToken { get; set; }
        public string? ExternalId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfilePicture { get; set; }
        public string? PhoneNo { get; set; }
    }

    public class LoginDto
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public string? Email { get; set; }
        public Guid UserId { get; set; }
        public string? UserType { get; set; }
        public bool? EmailVerified { get; set; }
        public bool? PhoneVerified { get; set; }
        public string? FullName { get; set; }
        public string? ProfileImage { get; set; }
        public string? IdAuthPassword { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

    }

}
