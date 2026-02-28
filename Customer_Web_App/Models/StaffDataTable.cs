using System.ComponentModel.DataAnnotations;

namespace Customer_Web_App.Models
{

    public class AddStaffDataTable
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email required")]
        public string? Email { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [StringLength(255, ErrorMessage = "Must be atleast 8 characters", MinimumLength = 8)]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])).{8,}$", ErrorMessage = "Passwords must contain uppercase,lowercase and number")]
        public string? Password { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Surname required")]
        public string? SurName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "FirstName required")]
        public string? FirstName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Staff Type required")]
        public string? AdminType { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Staff Code required")]
        public string? StaffCode { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Phone Number required")]
        public string? PhoneNo { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Sex required")]
        public string? Sex { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Address required")]
        public string? Address { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Branch required")]
        public string? Branch { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "City required")]
        public string? Locality { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "State required")]
        public string? State { get; set; }
    }

    public class AddMainStaffDataTable
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? SurName { get; set; }
        public string? FirstName { get; set; }
        public string? AdminType { get; set; }
        public string? StaffCode { get; set; }
        public string? PhoneNo { get; set; }
        public string? Sex { get; set; }
        public string? Address { get; set; }
        public string? Branch { get; set; }
        public string? Locality { get; set; }
        public string? State { get; set; }
    }
}
