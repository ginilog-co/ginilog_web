namespace Customer_Web_App.Models.LogisticsModel
{
    public class CompanyModelData
    {
        public Guid Id { get; set; }
        public Guid AdminId { get; set; }
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
        public List<string>? DeliveryTypes { get; set; }
        public List<string>? ServiceAreas { get; set; }
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
        public Guid CompanyModelDataId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class AddCompany
    {
        public string? ManagerId { get; set; }
        public string? CompanyEmail { get; set; }
        public string? CompanyName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CompanyRegNo { get; set; }
        public string? CompanyInfo { get; set; }
        public double ValueCharge { get; set; }
        public int NoOfTrucks { get; set; }
        public int NofOfBikes { get; set; }
        public string? CompanyAddress { get; set; }
       // public string? CompanyPostCodes { get; set; }
       // public string? CompanyLocality { get; set; }
       // public string? CompanyState { get; set; }
      //  public double CompanyLatitude { get; set; }
      //  public double CompanyLongitude { get; set; }
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public List<string>? DeliveryTypes { get; set; }
        public string? ServiceArea { get; set; }
        public IFormFile? LogoUpload { get; set; }
    }
    public class AddMainCompany
    {
        public string? ManagerId { get; set; }
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
        public string? CompanyAddress { get; set; }
        public string? PostCodes { get; set; }
        public string? Locality { get; set; }
        public string? State { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public List<string>? DeliveryTypes { get; set; }
        public List<string>? ServiceAreas { get; set; }
    }

    public class UpdateCompany
    {
        public string? CompanyEmail { get; set; }
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
        public List<string>? DeliveryTypes { get; set; }
        public List<string>? ServiceAreas { get; set; }
    }

    public class AllCompanyModelData
    {
        public CompanyModelData? CompanyModelData { get; set; }
        public List<OrderModelData>? OrderModelData { get; set; }
    }
}
