namespace Genilog_WebApi.Model.BookingsModel
{
    public class AccomodationDataModel
    {
        public Guid Id { get; set; }
        public Guid AdminId { get; set; }
        public string? AccomodationName { get; set; }
        public string? AccomodationLogo { get; set; }
        public string? AccomodationEmail { get; set; }
        public string? AccomodationDescription { get; set; }
        public string? AccomodationType { get; set; }
        public string? CheckInTime { get; set; }
        public string? CheckOutTime { get; set; }
        public string? AccomodationWebsite { get; set; }
        public string? AccomodationPhoneNo { get; set; }
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
        public string? AccomodationAdvertType { get; set; }
        public bool Available { get; set; }
        public AccomodationMondayModel? AccomodationMonday { get; set; }
        public AccomodationTuesdayModel? AccomodationTuesday { get; set; }
        public AccomodationWednesdayModel? AccomodationWednesday { get; set; }
        public AccomodationThursdayModel? AccomodationThursday { get; set; }
        public AccomodationFridayModel? AccomodationFriday { get; set; }
        public AccomodationSaturdayModel? AccomodationSaturday { get; set; }
        public AccomodationSundayModel? AccomodationSunday { get; set; }
        public List<string>? AccomodationImages { get; set; }
        public List<string>? AccomodationFacilities { get; set; }
        public List<AccomodationReviewModel>? AccomodationReviewModels { get; set; }
        public DateTime CreatedAt { get; set; }
    }
   

 

    // Accomodations Reviews
    public class AccomodationReviewModel
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? ProfileImage { get; set; }
        public string? ReviewMessage { get; set; }
        public double RatingNum { get; set; }
        public Guid UserId { get; set; }
        public AccomodationDataModel? AccomodationDataTables { get; set; }
        public Guid AccomodationDataTableId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Accomodation Data DTO

    public class AccomodationDataModelDto
    {
        public Guid Id { get; set; }
        public Guid AdminId { get; set; }
        public string? AccomodationName { get; set; }
        public string? AccomodationLogo { get; set; }
        public string? AccomodationEmail { get; set; }
        public string? AccomodationDescription { get; set; }
        public string? AccomodationType { get; set; }
        public string? CheckInTime { get; set; }
        public string? CheckOutTime { get; set; }
        public string? AccomodationWebsite { get; set; }
        public string? AccomodationPhoneNo { get; set; }
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
        public string? AccomodationAdvertType { get; set; }
        public bool Available { get; set; }
        public AccomodationMondayModelDto? AccomodationMonday { get; set; }
        public AccomodationTuesdayModelDto? AccomodationTuesday { get; set; }
        public AccomodationWednesdayModelDto? AccomodationWednesday { get; set; }
        public AccomodationThursdayModelDto? AccomodationThursday { get; set; }
        public AccomodationFridayModelDto? AccomodationFriday { get; set; }
        public AccomodationSaturdayModelDto? AccomodationSaturday { get; set; }
        public AccomodationSundayModelDto? AccomodationSunday { get; set; }
        public List<string>? AccomodationImages { get; set; }
        public List<string>? AccomodationFacilities { get; set; }
        public List<AccomodationReviewModelDto>? AccomodationReviewModels { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AccomodationReviewModelDto
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? ProfileImage { get; set; }
        public string? ReviewMessage { get; set; }
        public double RatingNum { get; set; }
        public Guid UserId { get; set; }
        public Guid AccomodationDataTableId { get; set; }
        public DateTime CreatedAt { get; set; }
    }


    // Add Accomodations
    public class AddAccomodation
    {
        public Guid ManagerId { get; set; }
        public string? AccomodationName { get; set; }
        public string? AccomodationLogo { get; set; }
        public string? AccomodationEmail { get; set; }
        public string? AccomodationAdvertType { get; set; }
        public string? AccomodationDescription { get; set; }
        public string? AccomodationType { get; set; }
        public string? CheckInTime { get; set; }
        public string? CheckOutTime { get; set; }
        public string? AccomodationWebsite { get; set; }
        public string? AccomodationPhoneNo { get; set; }
        public string? Location { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public double? BookingAmount { get; set; }
        public int? NoOfRooms { get; set; }
        public string? Locality { get; set; }
        public string? Postcode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public List<string>? AccomodationImages { get; set; }
        public List<string>? AccomodationFacilities { get; set; }
        public bool? Available { get; set; }
        public AddTimeSchedule? TimeSchedule { get; set; }

      
    }

    public class UpdateAccomodation
    {
        public string? AccomodationName { get; set; }
        public string? AccomodationEmail { get; set; }
        public string? AccomodationDescription { get; set; }
        public string? CheckInTime { get; set; }
        public string? CheckOutTime { get; set; }
        public string? AccomodationWebsite { get; set; }
        public string? AccomodationPhoneNo { get; set; }
        public string? Location { get; set; }
        public string? Locality { get; set; }
        public string? Postcode { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? NoOfRooms { get; set; }
        public string? AccomodationAdvertType { get; set; }
        public double? BookingAmount { get; set; }
        public bool? Available { get; set; }
        public List<string>? AccomodationImages { get; set; }
        public List<string>? AccomodationFacilities { get; set; }

    }

    public class AddAccomodationReview
    {
        public string? ReviewMessage { get; set; }
        public double RatingNum { get; set; }
    }
}
