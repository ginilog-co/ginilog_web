using System.ComponentModel.DataAnnotations;

namespace Genilog_WebApi.Model.AuthModel
{
    public class PhoneVerification
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Token required")]
        public string? Otp { get; set; }
    }
}
