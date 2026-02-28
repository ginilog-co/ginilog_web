using System.ComponentModel.DataAnnotations;

namespace Genilog_WebApi.Model.LogisticsModel
{
    public class RidersModelData
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? CompanyName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfilePicture { get; set; }
        public int Rating { get; set; }
        public bool IsVerified { get; set; }
        public bool Available { get; set; }
        public string? Address { get; set; }
        public string? PostCodes { get; set; }
        public string? Locality { get; set; }
        public string? State { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public List<RidersReviewModel>? RidersReviewModels { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class RidersReviewModel
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? ProfileImage { get; set; }
        public string? ReviewMessage { get; set; }
        public double RatingNum { get; set; }
        public Guid UserId { get; set; }
        public RidersModelData? RidersModelDatas { get; set; }
        public Guid RidersModelDataId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

  
    public class RidersModelDataDto
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? CompanyName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfilePicture { get; set; }
        public int Rating { get; set; }
        public bool IsVerified { get; set; }
        public bool Available { get; set; }
        public string? Address { get; set; }
        public string? PostCodes { get; set; }
        public string? Locality { get; set; }
        public string? State { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public List<RidersReviewModelDto>? RidersReviewModels { get; set; }
        public DateTime? CreatedAt { get; set; }
    }


    public class RidersReviewModelDto
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? ProfileImage { get; set; }
        public string? ReviewMessage { get; set; }
        public double? RatingNum { get; set; }
        public Guid UserId { get; set; }
        public Guid RidersModelDataId { get; set; }
        public DateTime CreatedAt { get; set; }
    }


    public class AddRiders
    {
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Email { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [StringLength(255, ErrorMessage = "Must be atleast 8 characters", MinimumLength = 8)]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])).{8,}$", ErrorMessage = "Passwords must contain uppercase,lowercase and number")]
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public int? Rating { get; set; }
        public bool? Available { get; set; }
        public string? State { get; set; }
        public string? Locality { get; set; }
        public string? Postcode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public string? Address { get; set; }
    }

    public class AddRidersReview
    {
        public string? ReviewMessage { get; set; }
        public double RatingNum { get; set; }
    }

 
    public class UpdateRiders
    {
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? CompanyName { get; set; }
        public string? PhoneNumber { get; set; }
        public int Rating { get; set; }
        // Region State
        public string? State { get; set; }
        public string? Locality { get; set; }
        public string? Address { get; set; }
        public string? Postcode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        // Bank Details
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        // Available
        public bool? Available { get; set; }
        // Verification
        public bool? IsVerified { get; set; }
        public string? ProfilePicture { get; set; }
    }

}
