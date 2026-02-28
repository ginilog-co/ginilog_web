using System.ComponentModel.DataAnnotations;

namespace Ginilog_AdminWeb.Models
{
    public class ResetPasswordRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Token is required")]
        public string? Token { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(255, ErrorMessage = "Must be atleast 8 characters", MinimumLength = 8)]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])).{8,}$", ErrorMessage = "Passwords must contain uppercase,lowercase and number")]
        public string? Password { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Confirm Password required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password Did not Match")]
        public string? ConfirmPassword { get; set; }
    }
}
