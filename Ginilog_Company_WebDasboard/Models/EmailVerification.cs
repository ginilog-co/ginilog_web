using System.ComponentModel.DataAnnotations;

namespace Ginilog_Company_WebDasboard.Models
{
    public class EmailVerification
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Token required")]
        public string? Token { get; set; }
        public string? Password { get; set; }
    }
}
