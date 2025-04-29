using System.ComponentModel.DataAnnotations;

namespace Ginilog_AdminWeb.Models
{
    public class StaffDataTable
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? SurName { get; set; }
        public string? FirstName { get; set; }
        public string? StaffType { get; set; }
        public string? StaffCode { get; set; }
        public string? ReferalCode { get; set; }
        public string? PhoneNo { get; set; }
        public string? Sex { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class AddStaffDataTable
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email required")]
        public string? Email { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Surname required")]
        public string? SurName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "FirstName required")]
        public string? FirstName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Staff Type required")]
        public string? StaffType { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Staff Code required")]
        public string? StaffCode { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Referral Code required")]
        public string? ReferalCode { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Phone Number required")]
        public string? PhoneNo { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Sex required")]
        public string? Sex { get; set; }
    }
}
