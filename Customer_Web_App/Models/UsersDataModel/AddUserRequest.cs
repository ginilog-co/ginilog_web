using System.ComponentModel.DataAnnotations;

namespace Customer_Web_App.Models.UsersDataModel
{
    public class AddUserRequest
    {
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        // Attribute to validate the email address
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "PhoneNo required")]
        [StringLength(11, ErrorMessage = "Must be 11 digit", MinimumLength = 11)]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Invalid Phone Number")]
        public string? PhoneNo { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [StringLength(255, ErrorMessage = "Must be atleast 8 characters", MinimumLength = 8)]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])).{8,}$", ErrorMessage = "Passwords must contain uppercase,lowercase and number")]
        public string? Password { get; set; }

        public string? IdToken { get; set; }
    }
    public class AddMainUserRequest
    {
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNo { get; set; }
        public string? Password { get; set; }
    }

    public class UpdateUserRequest
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "Sex required")]
        public string? Sex { get; set; }

    }

    public class UpdatesNames
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }

    public class UpdatesPhoneNo
    {
        public string? PhoneNo { get; set; }
    }
}
