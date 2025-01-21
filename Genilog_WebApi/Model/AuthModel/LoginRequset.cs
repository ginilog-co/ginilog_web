using System.ComponentModel.DataAnnotations;

namespace Genilog_WebApi.Model.AuthModel
{
    public class LoginRequset
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        public string? Email_PhoneNo { get; set; }
        public string? Password { get; set; }
    }

    public class LoginExternalRequset
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        public string? Email { get; set; }
        public string? ExternalId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfilePicture { get; set; }
        public string? PhoneNo { get; set; }
    }
    public class AddDeviceToken
    {
        public string? DeviceToken { get; set; }
    }
}
