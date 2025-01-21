using System.ComponentModel.DataAnnotations;

namespace Genilog_WebApi.Model.LogisticsModel
{
    public class CompanyModelData
    {
        public Guid Id { get; set; }
        public string? CompanyEmail { get; set; }
        public string? CompanyName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CompanyLogo { get; set; }
        public string? CompanyRegNo { get; set; }
        public string? CompanyInfo { get; set; }
        public int Rating { get; set; }
        public double ValueCharge { get; set; }
        public int NoOfTrucks { get; set; }
        public int NofOfBikes { get; set; }
        public bool Available { get; set; }
        public string? CompanyAddress { get; set; }
        public string? PostCodes { get; set; }
        public string? Locality { get; set; }
        public string? State { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public List<CompanyReviewModel>? CompanyReviewModels { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
    public class CompanyReviewModel
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? ProfileImage { get; set; }
        public string? ReviewMessage { get; set; }
        public double RatingNum { get; set; }
        public Guid UserId { get; set; }
        public CompanyModelData? CompanyModelDatas { get; set; }
        public Guid CompanyModelDataId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class CompanyModelDataDto
    {
        public Guid Id { get; set; }
        public string? CompanyEmail { get; set; }
        public string? CompanyName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CompanyLogo { get; set; }
        public string? CompanyRegNo { get; set; }
        public string? CompanyInfo { get; set; }
        public int Rating { get; set; }
        public double ValueCharge { get; set; }
        public int NoOfTrucks { get; set; }
        public int NofOfBikes { get; set; }
        public bool Available { get; set; }
        public string? CompanyAddress { get; set; }
        public string? PostCodes { get; set; }
        public string? Locality { get; set; }
        public string? State { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public List<CompanyReviewModelDto>? CompanyReviewModels { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
    public class CompanyReviewModelDto
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? ProfileImage { get; set; }
        public string? ReviewMessage { get; set; }
        public double RatingNum { get; set; }
        public Guid UserId { get; set; }
        public Guid CompanyModelDataId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class AddCompany
    {
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? CompanyEmail { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [StringLength(255, ErrorMessage = "Must be atleast 8 characters", MinimumLength = 8)]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])).{8,}$", ErrorMessage = "Passwords must contain uppercase,lowercase and number")]
        public string? Password { get; set; }
        public string? CompanyName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CompanyLogo { get; set; }
        public string? CompanyRegNo { get; set; }
        public string? CompanyInfo { get; set; }
        public int Rating { get; set; }
        public double ValueCharge { get; set; }
        public int NoOfTrucks { get; set; }
        public int NofOfBikes { get; set; }
        public string? CompanyAddress { get; set; }
        public string? PostCodes { get; set; }
        public string? Locality { get; set; }
        public string? State { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
    }
    public class UpdateCompany
    {
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? CompanyName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CompanyLogo { get; set; }
        public string? CompanyRegNo { get; set; }
        public string? CompanyInfo { get; set; }
        public int? Rating { get; set; }
        public double? ValueCharge { get; set; }
        public int? NoOfTrucks { get; set; }
        public int? NofOfBikes { get; set; }
        public string? CompanyAddress { get; set; }
        public string? PostCodes { get; set; }
        public string? Locality { get; set; }
        public string? State { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public bool? Available { get; set; }
    }
}
