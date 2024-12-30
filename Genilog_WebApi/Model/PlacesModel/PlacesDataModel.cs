
namespace Genilog_WebApi.Model.PlacesModel
{
    public class PlacesDataModel
    {
        public Guid Id { get; set; }
        public Guid AdminId { get; set; }
        public string? PlaceName { get; set; }
        public string? PlaceLogo { get; set; }
        public string? PlaceEmail { get; set; }
        public string? PlaceOverview { get; set; }
        public string? PlacesAdditionalInfo { get; set; }
        public string? CancelationPolicy { get; set; }
        public string? PlacesHighlights { get; set; }
        public string? PlaceType { get; set; }
        public string? CheckInTime { get; set; }
        public string? CheckOutTime { get; set; }
        public string? PlaceWebsite { get; set; }
        public string? PlacePhoneNo { get; set; }
        public string? Location { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Locality { get; set; }
        public string? Postcode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double BookingAmount { get; set; }
        public double Rating { get; set; }
        public string? PlaceAdvertType { get; set; }
        public string? TicketType { get; set; }
        public bool IsPayment {  get; set; }
        public bool Available { get; set; }
        public PlacesMondayModel? PlacesMonday { get; set; }
        public PlacesTuesdayModel? PlacesTuesday { get; set; }
        public PlacesWednesdayModel? PlacesWednesday { get; set; }
        public PlacesThursdayModel? PlacesThursday { get; set; }
        public PlacesFridayModel? PlacesFriday { get; set; }
        public PlacesSaturdayModel? PlacesSaturday { get; set; }
        public PlacesSundayModel? PlacesSunday { get; set; }
        public List<PlaceImages>? PlaceImages { get; set; }
        public List<PlaceFacilities>? PlaceFacilities { get; set; }
        public List<PlaceWhatToExpect>? PlaceWhatToExpects { get; set; }
        public List<PlaceReviewModel>? PlaceReviewModels { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Places Images
    public class PlaceImages
    {
        public Guid Id { get; set; }
        public string? ImagePath { get; set; }
        public PlacesDataModel? PlacesDataModels { get; set; }
        public Guid PlacesDataModelId { get; set; }
    }

    // Places Facilities
    public class PlaceFacilities
    {
        public Guid Id { get; set; }
        public string? Facilities { get; set; }
        public PlacesDataModel? PlacesDataModels { get; set; }
        public Guid PlacesDataModelId { get; set; }
    }

    // Places What to Expect
    public class PlaceWhatToExpect
    {
        public Guid Id { get; set; }
        public string? Titles { get; set; }
        public string? Description { get; set; }
        public string? SubTitle { get; set; }
        public PlacesDataModel? PlacesDataModels { get; set; }
        public Guid PlacesDataModelId { get; set; }
    }

    // Places Review
    public class PlaceReviewModel
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? ProfileImage { get; set; }
        public string? ReviewMessage { get; set; }
        public double RatingNum { get; set; }
        public Guid UserId { get; set; }
        public PlacesDataModel? PlacesDataModels { get; set; }
        public Guid PlacesDataModelId { get; set; }
        public DateTime CreatedAt { get; set; }
    }


    // Places Data DTO
    public class PlacesDataModelDto
    {
        public Guid Id { get; set; }
        public Guid AdminId { get; set; }
        public string? PlaceName { get; set; }
        public string? PlaceLogo { get; set; }
        public string? PlaceEmail { get; set; }
        public string? PlaceOverview { get; set; }
        public string? PlacesAdditionalInfo { get; set; }
        public string? CancelationPolicy { get; set; }
        public string? PlacesHighlights { get; set; }
        public string? PlaceType { get; set; }
        public string? CheckInTime { get; set; }
        public string? CheckOutTime { get; set; }
        public string? PlaceWebsite { get; set; }
        public string? PlacePhoneNo { get; set; }
        public string? Location { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Locality { get; set; }
        public string? Postcode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double BookingAmount { get; set; }
        public double Rating { get; set; }
        public string? PlaceAdvertType { get; set; }
        public string? TicketType { get; set; }
        public bool IsPayment { get; set; }
        public bool Available { get; set; }
        public PlacesMondayModelDto? PlacesMonday { get; set; }
        public PlacesTuesdayModelDto? PlacesTuesday { get; set; }
        public PlacesWednesdayModel? PlacesWednesday { get; set; }
        public PlacesThursdayModelDto? PlacesThursday { get; set; }
        public PlacesFridayModelDto? PlacesFriday { get; set; }
        public PlacesSaturdayModelDto? PlacesSaturday { get; set; }
        public PlacesSundayModelDto? PlacesSunday { get; set; }
        public List<PlaceImagesDto>? PlaceImages { get; set; }
        public List<PlaceFacilitiesDto>? PlaceFacilities { get; set; }
        public List<PlaceWhatToExpectDto>? PlaceWhatToExpects { get; set; }
        public List<PlaceReviewModelDto>? PlaceReviewModels { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class PlaceImagesDto
    {
        public Guid Id { get; set; }
        public string? ImagePath { get; set; }
        public Guid PlacesDataModelId { get; set; }
    }
    public class PlaceFacilitiesDto
    {
        public Guid Id { get; set; }
        public string? Facilities { get; set; }
        public Guid PlacesDataModelId { get; set; }
    }
    public class PlaceWhatToExpectDto
    {
        public Guid Id { get; set; }
        public string? Titles { get; set; }
        public string? Description { get; set; }
        public string? SubTitle { get; set; }
        public Guid PlacesDataModelId { get; set; }
    }
    public class PlaceReviewModelDto
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? ProfileImage { get; set; }
        public string? ReviewMessage { get; set; }
        public double RatingNum { get; set; }
        public Guid UserId { get; set; }
        public Guid PlacesDataModelId { get; set; }
        public DateTime CreatedAt { get; set; }
    }


    // Add Places
    public class AddPlaces
    {
        public string? PlaceName { get; set; }
        public string? PlaceEmail { get; set; }
        public string? PlaceOverview { get; set; }
        public string? PlacesAdditionalInfo { get; set; }
        public string? CancelationPolicy { get; set; }
        public string? PlacesHighlights { get; set; }
        public string? PlaceType { get; set; }
        public string? CheckInTime { get; set; }
        public string? CheckOutTime { get; set; }
        public string? PlaceWebsite { get; set; }
        public string? PlacePhoneNo { get; set; }
        public string? Location { get; set; }
        public string? Locality { get; set; }
        public string? Postcode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public int BookingAmount { get; set; }
        public int Rating { get; set; }
        public string? PlaceAdvertType { get; set; }
        public string? TicketType { get; set; }
        public bool IsPayment { get; set; }
        public string? PlaceLogo { get; set; }
        public List<string>? PlacesImages { get; set; }
        public AddTimeSchedule? TimeSchedule { get; set; }
    }
    public class UpdatePlaces
    {
        public string? PlaceName { get; set; }
        public string? PlaceEmail { get; set; }
        public string? CheckInTime { get; set; }
        public string? CheckOutTime { get; set; }
        public string? PlaceWebsite { get; set; }
        public string? PlacePhoneNo { get; set; }
        public string? Location { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Locality { get; set; }
        public string? Postcode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? PlaceAdvertType { get; set; }
        public double? BookingAmount { get; set; }
        public string? PlaceOverview { get; set; }
        public string? PlacesAdditionalInfo { get; set; }
        public string? CancelationPolicy { get; set; }
        public string? PlacesHighlights { get; set; }
        public bool? IsPayment { get; set; }
        public bool? Available { get; set; }
        public string? TicketType { get; set; }
        public string? PlaceType { get; set; }
    }
   

    public class AddPlacesFacilities
    {
        public string? Facilities { get; set; }
    }
    public class AddPlaceWhatToExpect
    {
        public string? Titles { get; set; }
        public string? Description { get; set; }
        public string? SubTitle { get; set; }
    }
    public class AddPlacesReview
    {
        public string? ReviewMessage { get; set; }
    }
}
