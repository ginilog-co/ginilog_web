using System.ComponentModel.DataAnnotations;

namespace Genilog_WebApi.Model.LogisticsModel
{
    public class LogisticsDataModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? CompanyName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? LogisticsLogo { get; set; }
        public int Rating { get; set; }
        public double ValueCharge { get; set; }
        public string? CompanyAddress { get; set; }
        public string? CompanyRegNo { get; set; }
        public string? CompanyInfo { get; set; }
        public int NoOfTrucks { get; set; }
        public int NofOfBikes { get; set; }
        public bool? IsVerified { get; set; }
        public bool? IsIndividual { get; set; }
        public bool Available { get; set; }
        public string? IdCardUpload { get; set; }
        public bool IdCardVerification { get; set; }
        public string? LicenseUpload { get; set; }
        public bool LicenseVerification { get; set; }
        public string? Address { get; set; }
        public string? PostCodes { get; set; }
        public string? Locality { get; set; }
        public string? State { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public List<LogisticsReviewModel>? LogisticsReviewModels { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class LogisticsReviewModel
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? ProfileImage { get; set; }
        public string? ReviewMessage { get; set; }
        public double RatingNum { get; set; }
        public Guid UserId { get; set; }
        public LogisticsDataModel? LogisticsDataModels { get; set; }
        public Guid LogisticsDataModelId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

  
    public class LogisticsDataModelDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? CompanyName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? LogisticsLogo { get; set; }
        public int Rating { get; set; }
        public double ValueCharge { get; set; }
        public string? CompanyAddress { get; set; }
        public string? CompanyRegNo { get; set; }
        public string? CompanyInfo { get; set; }
        public int NoOfTrucks { get; set; }
        public int NofOfBikes { get; set; }
        public bool? IsVerified { get; set; }
        public bool? IsIndividual { get; set; }
        public bool Available { get; set; }
        public string? IdCardUpload { get; set; }
        public bool IdCardVerification { get; set; }
        public string? LicenseUpload { get; set; }
        public bool LicenseVerification { get; set; }
        public string? Address { get; set; }
        public string? PostCodes { get; set; }
        public string? Locality { get; set; }
        public string? State { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public List<LogisticsReviewModelDto>? LogisticsReviewModels { get; set; }
        public DateTime? CreatedAt { get; set; }
    }


    public class LogisticsReviewModelDto
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? ProfileImage { get; set; }
        public string? ReviewMessage { get; set; }
        public double RatingNum { get; set; }
        public Guid UserId { get; set; }
        public Guid LogisticsDataModelId { get; set; }
        public DateTime CreatedAt { get; set; }
    }


    public class AddLogistics
    {
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Email { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [StringLength(255, ErrorMessage = "Must be atleast 8 characters", MinimumLength = 8)]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])).{8,}$", ErrorMessage = "Passwords must contain uppercase,lowercase and number")]
        public string? Password { get; set; }
        public string? CompanyName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? LogisticsLogoUrl { get; set; }
        public string? IdCardUploadUrl { get; set; }
        public string? LicenseUploadUrl { get; set; }
        public int Rating { get; set; }
        public double ValueCharge { get; set; }
        public string? CompanyAddress { get; set; }
        public string? CompanyRegNo { get; set; }
        public string? CompanyInfo { get; set; }
        public int NoOfTrucks { get; set; }
        public int NofOfBikes { get; set; }
        public bool? IsIndividual { get; set; }
        public bool Available { get; set; }
        public string? State { get; set; }
        public string? Locality { get; set; }
        public string? Postcode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
    }

    public class AddLogisticsReview
    {
        public string? ReviewMessage { get; set; }
        public double RatingNum { get; set; }
    }

 
    public class UpdateLogistics
    {
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? CompanyName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CompanyAddress { get; set; }
        public int Rating { get; set; }
        public double ValueCharge { get; set; }
        public int NoOfTrucks { get; set; }
        public int NofOfBikes { get; set; }
        // Region State
        public string? State { get; set; }
        public string? Locality { get; set; }
        public string? Location { get; set; }
        public string? Postcode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        // Bank Details
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        // Available
        public bool Available { get; set; }
        // Verification
        public bool IdCardVerification { get; set; }
        public bool LicenseVerification { get; set; }
        public bool? IsVerified { get; set; }
    }

}
