using System.ComponentModel.DataAnnotations;

namespace Genilog_WebApi.Model.UsersDataModel
{
    public class AddUserRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "LastName is required")]
        public string? LastName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "FirstName is required")]
        public string? FirstName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        // Attribute to validate the email address
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "PhoneNo required")]
       // [StringLength(11, ErrorMessage = "Must be 10 digit", MinimumLength = 10)]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Invalid Phone Number")]
        public string? PhoneNo { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [StringLength(255, ErrorMessage = "Must be atleast 8 characters", MinimumLength = 8)]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])).{8,}$", ErrorMessage = "Passwords must contain uppercase,lowercase and number")]
        public string? Password { get; set; }
    }

    public class UpdateUserRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNo { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Sex { get; set; }
        public bool? UserStatus { get; set; }
        public string? Address { get; set; }
        public string? PostCodes { get; set; }
        public string? Locality { get; set; }
        public string? State { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

    }

    public class UpdateMoneyBox
    {
        public double MoneyBoxBalance { get; set; }
    }
}
