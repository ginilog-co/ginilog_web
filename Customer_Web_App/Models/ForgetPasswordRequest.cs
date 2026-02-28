using System.ComponentModel.DataAnnotations;

namespace Customer_Web_App.Models
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
