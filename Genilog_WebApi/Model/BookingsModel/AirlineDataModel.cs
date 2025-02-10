

namespace Genilog_WebApi.Model.BookingsModel
{
    public class AirlineDataModel
    {
        public Guid Id { get; set; }
        public Guid AdminId { get; set; }
        public string? AirlineName { get; set; }
        public string? AirlineLogo { get; set; }
        public string? AirlineEmail { get; set; }
        public string? AirlineInfo { get; set; }
        public string? AirlineType { get; set; }
        public string? AirlinePhoneNo { get; set; }
        public string? AirlineWebsite { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Locality { get; set; }
        public double BookingAmount { get; set; }
        public double Rating { get; set; }
       
        public bool Available { get; set; }
        public List<AirlineImages>? AirlineImages { get; set; }
        public List<AirCraftList>? AirCraftList { get; set; }
        public List<AirLineServiceLocation>? AirLineServiceLocations { get; set; }
        public List<AirlinePayment>? AirlinePayments { get; set; }
        public List<AirlineReviewModel>? AirlineReviewModels { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Places Images
    public class AirlineImages
    {
        public Guid Id { get; set; }
        public string? ImagePath { get; set; }
        public AirlineDataModel? AirlineDataModels { get; set; }
        public Guid AirlineDataModelId { get; set; }
    }

    // AirCraft List
    public class AirCraftList
    {
        public Guid Id { get; set; }
        public string? Model { get; set; }
        public string? Manufacturer { get; set; }
        public int Capacity { get; set; }
        public AirlineDataModel? AirlineDataModels { get; set; }
        public Guid AirlineDataModelId { get; set; }
    }
    // AirLine Service Location
    public class AirLineServiceLocation
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public AirlineDataModel? AirlineDataModels { get; set; }
        public Guid AirlineDataModelId { get; set; }
    }

    // AirCraft Payment
    public class AirlinePayment
    {
        public Guid Id { get; set; }
        public string? Titles { get; set; }
        public double Amount { get; set; }
        public AirlineDataModel? AirlineDataModels { get; set; }
        public Guid AirlineDataModelId { get; set; }
    }

    // Places Review
    public class AirlineReviewModel
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? ProfileImage { get; set; }
        public string? ReviewMessage { get; set; }
        public double RatingNum { get; set; }
        public Guid UserId { get; set; }
        public AirlineDataModel? AirlineDataModels { get; set; }
        public Guid AirlineDataModelId { get; set; }
        public DateTime CreatedAt { get; set; }
    }


    // Places Data DTO
    public class AirlineDataModelDto
    {
        public Guid Id { get; set; }
        public Guid AdminId { get; set; }
        public string? AirlineName { get; set; }
        public string? AirlineLogo { get; set; }
        public string? AirlineEmail { get; set; }
        public string? AirlineInfo { get; set; }
        public string? AirlineType { get; set; }
        public string? AirlinePhoneNo { get; set; }
        public string? AirlineWebsite { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Locality { get; set; }
        public double BookingAmount { get; set; }
        public double Rating { get; set; }
       
        public bool Available { get; set; }
        public List<AirlineImagesDto>? AirlineImages { get; set; }
        public List<AirCraftListDto>? AirCraftList { get; set; }
        public List<AirLineServiceLocationDto>? AirLineServiceLocations { get; set; }
        public List<AirlinePaymentDto>? AirlinePayments { get; set; }
        public List<AirlineReviewModelDto>? AirlineReviewModels { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class AirlineImagesDto
    {
        public Guid Id { get; set; }
        public string? ImagePath { get; set; }
        public Guid AirlineDataModelId { get; set; }
    }
    public class AirCraftListDto
    {
        public Guid Id { get; set; }
        public string? Model { get; set; }
        public string? Manufacturer { get; set; }
        public int Capacity { get; set; }
        public Guid AirlineDataModelId { get; set; }
    }
    // AirLine Service Location
    public class AirLineServiceLocationDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public Guid AirlineDataModelId { get; set; }
    }
    public class AirlinePaymentDto
    {
        public Guid Id { get; set; }
        public string? Titles { get; set; }
        public double Amount { get; set; }
        public Guid AirlineDataModelId { get; set; }
    }
    public class AirlineReviewModelDto
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? ProfileImage { get; set; }
        public string? ReviewMessage { get; set; }
        public double RatingNum { get; set; }
        public Guid UserId { get; set; }
        public Guid AirlineDataModelId { get; set; }
        public DateTime CreatedAt { get; set; }
    }


    // Add Places
    public class AddAirlines
    {
        public string? AirlineName { get; set; }
        public string? AirlineLogo { get; set; }
        public string? AirlineEmail { get; set; }
        public string? AirlineInfo { get; set; }
        public string? AirlineType { get; set; }
        public string? AirlinePhoneNo { get; set; }
        public string? AirlineWebsite { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Locality { get; set; }
        public double BookingAmount { get; set; }
        public double? Rating { get; set; }
       
        public bool? Available { get; set; }
        public List<string>? AirlineImages { get; set; }
    }
    public class UpdateAirlines
    {
        public string? AirlineName { get; set; }
        public string? AirlineLogo { get; set; }
        public string? AirlineEmail { get; set; }
        public string? AirlineInfo { get; set; }
        public string? AirlineType { get; set; }
        public string? AirlinePhoneNo { get; set; }
        public string? AirlineWebsite { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Locality { get; set; }
        public double? BookingAmount { get; set; }
        public double? Rating { get; set; }
       
        public bool? Available { get; set; }
    }


    public class AddAirCraft
    {
        public string? Model { get; set; }
        public string? Manufacturer { get; set; }
        public int Capacity { get; set; }
    }
    public class AddAirlinePayment
    {
        public string? Titles { get; set; }
        public double Amount { get; set; }
        
    }
    public class AddAirlinesServiceLocation
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }
    public class AddAirlinesReview
    {
        public string? ReviewMessage { get; set; }
    } 
    public class AddAirlinesImages
    {
        public List<string>? AirlineImages { get; set; }
    }
}
