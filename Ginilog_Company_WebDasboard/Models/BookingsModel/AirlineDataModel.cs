namespace Ginilog_Company_WebDasboard.Models.BookingsModel
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
        public List<string>? AirlineImages { get; set; }
        public List<AirCraftList>? AirCraftList { get; set; }
        public List<AirLineServiceLocation>? AirLineServiceLocations { get; set; }
        public List<AirlineReviewModel>? AirlineReviewModels { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class AirCraftList
    {
        public Guid Id { get; set; }
        public string? Model { get; set; }
        public string? Manufacturer { get; set; }
        public int Capacity { get; set; }
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
        public Guid AirlineDataModelId { get; set; }
    }
    public class AirlineReviewModel
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
        public List<string>? AirlineImages { get; set; }
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
