namespace Customer_Web_App.Models.BookingsModel
{
    public class AllTimeSchedule
    {
    }

    // Accomodation Time Schedule
   
    public class AccomodationMondayModel
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public Guid AccomodationDataModelId { get; set; }
    }
    public class AccomodationTuesdayModel
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public Guid AccomodationDataModelId { get; set; }
    }
    public class AccomodationWednesdayModel
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public Guid AccomodationDataModelId { get; set; }
    }
    public class AccomodationThursdayModel
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public Guid AirlineDataModelId { get; set; }
    }
    public class AccomodationFridayModel
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public Guid AccomodationDataModelId { get; set; }
    }
    public class AccomodationSaturdayModel
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public Guid AccomodationDataModelId { get; set; }
    }
    public class AccomodationSundayModel
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public Guid AccomodationDataModelId { get; set; }
    }


    public class AddTimeSchedule
    {
        public HourModel? Monday { get; set; }
        public HourModel? Tuesday { get; set; }
        public HourModel? Wednesday { get; set; }
        public HourModel? Thursday { get; set; }
        public HourModel? Friday { get; set; }
        public HourModel? Saturday { get; set; }
        public HourModel? Sunday { get; set; }
    }
    public class HourModel
    {
        public TimeSpan? HourStart { get; set; }
        public TimeSpan? HourEnd { get; set; }
        public string? Start { get; set; }
        public string? End { get; set; }
        public bool IsClosed { get; set; }
    }
}
