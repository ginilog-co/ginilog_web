using System.ComponentModel.DataAnnotations;

namespace Customer_Web_App.Models
{
    public class EmailVerification
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Token required")]
        public string? Token { get; set; }
        public string? Password { get; set; }
    }
    public class ResendEmailVerification
    {
        public string? Email { get; set; }
    }
}
