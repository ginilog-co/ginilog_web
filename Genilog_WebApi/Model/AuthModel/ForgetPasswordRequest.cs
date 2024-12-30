using System.ComponentModel.DataAnnotations;

namespace Genilog_WebApi.Model.AuthModel
{
    public class ForgetPasswordRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }
    }
}
