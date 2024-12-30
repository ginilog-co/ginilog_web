using System.ComponentModel.DataAnnotations;

namespace Genilog_WebApi.Model.AuthModel
{
    public class EmailVerification
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Token required")]
        public string? Token { get; set; }
        public string? Password { get; set; }
    }
}
