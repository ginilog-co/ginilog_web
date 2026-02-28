using System.ComponentModel.DataAnnotations;

namespace Genilog_WebApi.Model.AuthModel
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Token { get; set; } = string.Empty;
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [StringLength(255, ErrorMessage = "Must be atleast 8 characters", MinimumLength = 8)]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])).{8,}$", ErrorMessage = "Passwords must contain uppercase,lowercase and number")]
        public string Password { get; set; } = string.Empty;
    }
}
