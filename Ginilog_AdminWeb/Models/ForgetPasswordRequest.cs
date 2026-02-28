using System.ComponentModel.DataAnnotations;

namespace Ginilog_AdminWeb.Models
{
    public class ForgetPasswordRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }
    }

    public class ForgetPasswordRequestDto
    {
        public string? Email { get; set; }
    }
}
