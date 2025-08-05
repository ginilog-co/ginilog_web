using Ginilog_Company_WebDasboard.Models.BookingsModel;
using Ginilog_Company_WebDasboard.Models.LogisticsModel;
using System.ComponentModel.DataAnnotations;

namespace Ginilog_Company_WebDasboard.Models
{
    public class AdminModelTable
    {

        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? SurName { get; set; }
        public string? FirstName { get; set; }
        public string? StaffCode { get; set; }
        public string? PhoneNo { get; set; }
        public string? ImagePath { get; set; }
        public string? Sex { get; set; }
        public string? State { get; set; }
        public string? Locality { get; set; }
        public string? Address { get; set; }
        public string? Branch { get; set; }
        public string? AdminType { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyUserName { get; set; }
        public List<string>? CompanyType { get; set; }
        public Guid ManagerId { get; set; }
        public string? DatePublished { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class AddAdminRequest
    {
        public string? FirstName { get; set; }
        public string? SurName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        // Attribute to validate the email address
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [StringLength(255, ErrorMessage = "Must be atleast 8 characters", MinimumLength = 8)]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])).{8,}$", ErrorMessage = "Passwords must contain uppercase,lowercase and number")]
        public string? Password { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Sex required")]
        public string? Sex { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Staff Code required")]
        public string? StaffCode { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Phone Number required")]
        public string? PhoneNo { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Address Number required")]
        public string? Address { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Branch Number required")]
        public string? Branch { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Company Name required")]
        public string? CompanyName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Company UserName required")]
        public string? CompanyType { get; set; }
    }
    public class AddMainAdminRequest
    {
        public string? AdminType { get; set; }
        public string? FirstName { get; set; }
        public string? SurName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Sex { get; set; }
        public string? StaffCode { get; set; }
        public string? PhoneNo { get; set; }
        public string? State { get; set; }
        public string? Locality { get; set; }
        public string? Address { get; set; }
        public string? Branch { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyUserName { get; set; }
        public List<string>? CompanyType { get; set; }
    }

    public class UpdateAdminRequest
    {
        public string? SurName { get; set; }
        public string? FirstName { get; set; }
        public string? StaffCode { get; set; }
        public string? PhoneNo { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? Branch { get; set; }
    }

    public class ProfileData
    {
        public AdminModelTable? AdminModelTable { get; set; } 
        public CompanyModelData? CompanyModelData { get; set; }
        public AccomodationDataModel? AccomodationDataModel { get; set; }

    }


}
