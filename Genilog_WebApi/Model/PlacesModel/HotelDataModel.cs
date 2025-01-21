
namespace Genilog_WebApi.Model.PlacesModel
{
    public class HotelDataModel
    {
        public Guid Id { get; set; }
        public Guid AdminId { get; set; }
        public string? HotelName { get; set; }
        public string?  HotelLogo   { get; set; }
        public string? HotelEmail { get; set; }
        public string? HotelDescription { get; set; }
        public string? HotelType { get; set; }
        public string? CheckInTime { get; set; }
        public string? CheckOutTime {  get; set; }
        public string? HotelWebsite {  get; set; }
        public string? HotelPhoneNo { get; set; }
        public string? Location { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Locality { get; set; }
        public string? Postcode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double BookingAmount { get; set; }
        public double Rating { get; set; }
        public int NoOfRooms { get; set; }
        public string? HotelAdvertType { get; set; }
        public bool Available { get; set; }
        public HotelMondayModel? HotelMonday { get; set; }
        public HotelTuesdayModel? HotelTuesday { get; set; }
        public HotelWednesdayModel? HotelWednesday { get; set; }
        public HotelThursdayModel? HotelThursday { get; set; }
        public HotelFridayModel? HotelFriday { get; set; }
        public HotelSaturdayModel? HotelSaturday { get; set; }
        public HotelSundayModel? HotelSunday { get; set; }
        public List<HotelImages>? HotelImages { get; set; }
        public List<HotelFacilities>? HotelFacilities { get; set; }
        public List<HotelReviewModel>? HotelReviewModels { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    // Hotel Images
    public class HotelImages
    {
        public Guid Id { get; set; }
        public string? ImagePath { get; set; }
        public HotelDataModel? HotelDataTables { get; set; }
        public Guid HotelDataTableId { get; set; }
    }

    // Hotel Facilities
    public class HotelFacilities
    {
        public Guid Id { get; set; }
        public string? Facilities { get; set; }
        public HotelDataModel? HotelDataTables { get; set; }
        public Guid HotelDataTableId { get; set; }
    }

    // Hotels Reviews
    public class HotelReviewModel
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? ProfileImage { get; set; }
        public string? ReviewMessage { get; set; }
        public double RatingNum { get; set; }
        public Guid UserId { get; set; }
        public HotelDataModel? HotelDataTables { get; set; }
        public Guid HotelDataTableId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Hotel Data DTO

    public class HotelDataModelDto
    {
        public Guid Id { get; set; }
        public Guid AdminId { get; set; }
        public string? HotelName { get; set; }
        public string? HotelLogo { get; set; }
        public string? HotelEmail { get; set;}
        public string? HotelDescription { get; set; }
        public string? HotelType { get; set; }
        public string? CheckInTime { get; set; }
        public string? CheckOutTime { get; set; }
        public string? HotelWebsite { get; set; }
        public string? HotelPhoneNo { get; set; }
        public string? Location { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Locality { get; set; }
        public string? Postcode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double BookingAmount { get; set; }
        public double Rating { get; set; }
        public int NoOfRooms { get; set; }
        public string? HotelAdvertType { get; set; }
        public bool Available { get; set; }
        public HotelMondayModelDto? HotelMonday { get; set; }
        public HotelTuesdayModelDto? HotelTuesday { get; set; }
        public HotelWednesdayModelDto? HotelWednesday { get; set; }
        public HotelThursdayModelDto? HotelThursday { get; set; }
        public HotelFridayModelDto? HotelFriday { get; set; }
        public HotelSaturdayModelDto? HotelSaturday { get; set; }
        public HotelSundayModelDto? HotelSunday { get; set; }
        public List<HotelImagesDto>? HotelImages { get; set; }
        public List<HotelFacilitiesDto>? HotelFacilities { get; set; }
        public List<HotelReviewModelDto>? HotelReviewModels { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class HotelImagesDto
    {
        public Guid Id { get; set; }
        public string? ImagePath { get; set; }
        public Guid HotelDataTableId { get; set; }
    }
    public class HotelFacilitiesDto
    {
        public Guid Id { get; set; }
        public string? Facilities { get; set; }
        public Guid HotelDataTableId { get; set; }
    }
    public class HotelReviewModelDto
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? ProfileImage { get; set; }
        public string? ReviewMessage { get; set; }
        public double RatingNum { get; set; }
        public Guid UserId { get; set; }
        public Guid HotelDataTableId { get; set; }
        public DateTime CreatedAt { get; set; }
    }


    // Add Hotels
    public class AddHotel
    {
        public string? HotelName { get; set; }
        public string? HotelLogo { get; set; }
        public string? HotelEmail { get; set; }
        public string? HotelDescription { get; set; }
        public string? HotelType { get; set; }
        public string? CheckInTime { get; set; }
        public string? CheckOutTime { get; set; }
        public string? HotelWebsite { get; set; }
        public string? HotelPhoneNo { get; set; }
        public string? Location { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public double BookingAmount { get; set; }
        public int NoOfRooms { get; set; }
        public string? Locality { get; set; }
        public string? Postcode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<string>? HotelImages { get; set;}
        public AddTimeSchedule? TimeSchedule { get; set; }
    }

    public class UpdateHotel
    {
        public string? HotelName { get; set; }
        public string? HotelEmail { get; set; }
        public string? HotelDescription { get; set; }
        public string? CheckInTime { get; set; }
        public string? CheckOutTime { get; set; }
        public string? HotelWebsite { get; set; }
        public string? HotelPhoneNo { get; set; }
        public string? Location { get; set; }
        public string? Locality { get; set; }
        public string? Postcode { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? NoOfRooms { get; set; }
        public string? HotelAdvertType { get; set; }
        public double? BookingAmount { get; set; }
        public bool? Available { get; set; }

    }

    public class AddHotelFacilities
    {
        public string? Facilities { get; set; }
    }

    public class AddHotelReview
    {
        public string? ReviewMessage { get; set; }
    }
}
