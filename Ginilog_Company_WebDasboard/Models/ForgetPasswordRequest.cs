using System.ComponentModel.DataAnnotations;

namespace Ginilog_Company_WebDasboard.Models
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
