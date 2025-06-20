namespace Genilog_WebApi.Model.BookingsModel
{
    public class BookAccomodationReservatioModel
    {
        public Guid Id { get; set; }
        public Guid AccomodationId { get; set; }
        public Guid AdminId { get; set; }
        public string? AccomodationName { get; set; }
        public string? AccomodationType { get; set; }
        public string? AccomodationLocality { get; set; }
        public string? AccomodationState { get; set; }
        public string? AccomodationImage { get; set; }
        public string? TicketNum { get; set; }
        public int RoomNumber { get; set; }
        public int? MaximumNoOfGuest { get; set; }
        public double RoomPrice { get; set; }
        public string? RoomType { get; set; }
        public List<string>? RoomImages { get; set; }
        public List<string>? RoomFeatures { get; set; }
        public string? QRCode { get; set; }
        public bool? IsBooked { get; set; }
        public DateTime UpdateddAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CheckInTime { get; set; }
        public string? CheckOutTime { get; set; }
        public string? Location { get; set; }
    }
    public class BookAccomodationReservatioModelDto
    {
        public Guid Id { get; set; }
        public Guid AccomodationId { get; set; }
        public Guid AdminId { get; set; }
        public string? AccomodationName { get; set; }
        public string? AccomodationType { get; set; }
        public string? AccomodationLocality { get; set; }
        public string? AccomodationState { get; set; }
        public string? AccomodationImage { get; set; }
        public string? TicketNum { get; set; }
        public int RoomNumber { get; set; }
        public int? MaximumNoOfGuest { get; set; }
        public double RoomPrice { get; set; }
        public string? RoomType { get; set; }
        public List<string>? RoomImages { get; set; }
        public List<string>? RoomFeatures { get; set; }
        public string? QRCode { get; set; }
        public bool? IsBooked { get; set; }
        public DateTime UpdateddAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CheckInTime { get; set; }
        public string? CheckOutTime { get; set; }
        public string? Location { get; set; }
    }

    public class AddBookAccomodationReservation
    {
        public int? RoomNumber { get; set; }
        public int? MaximumNoOfGuest { get; set; }
        public double? RoomPrice { get; set; }
        public string? RoomType { get; set; }
        public List<string>? RoomImages { get; set; }
        public List<string>? RoomFeatures { get; set; }
        public bool? IsBooked { get; set; }
      
    }

    // Customer
    public class CustomerBookedReservation
    {
        public Guid Id { get; set; }
        public Guid ResevationId { get; set; }
        public Guid AccomodationId { get; set; }
        public Guid AdminId { get; set; }
        public string? AccomodationName { get; set; }
        public string? AccomodationType { get; set; }
        public string? AccomodationLocation { get; set; }
        public string? AccomodationImage { get; set; }
        public Guid UserId { get; set; }
        public int RoomNumber{ get; set; }
        public string? QRCode { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhoneNumber { get; set; }
        public string? CustomerEmail { get; set; }
        public int NumberOfGuests { get; set; }
        public string? TrnxReference { get; set; }
        public string? PaymentChannel { get; set; }
        public bool PaymentStatus { get; set; }
        public string? Comment { get; set; }
        public string? TicketNum { get; set; }
        public DateTime ReservationStartDate { get; set; }
        public DateTime ReservationEndDate { get; set; }
        public int? NoOfDays { get; set; }
        public bool TicketClosed { get; set; }
        public double? TotalCost { get; set; }
        public DateTime UpdateddAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid StaffId { get; set; }
        public string? StaffName { get; set; }
        public string? PurchaseChannel { get; set; }
        public string? UserType { get; set; } // e.g., "Registerd", "Not Registered"
    }
    public class CustomerBookedReservationDto
    {
        public Guid Id { get; set; }
        public Guid ResevationId { get; set; } 
        public Guid AccomodationId { get; set; }
        public Guid AdminId { get; set; }
        public string? AccomodationName { get; set; }
        public string? AccomodationType { get; set; }
        public string? AccomodationLocation { get; set; }
        public string? AccomodationImage { get; set; }
        public Guid UserId { get; set; }
        public int RoomNumber { get; set; }
        public string? QRCode { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhoneNumber { get; set; }
        public string? CustomerEmail { get; set; }
        public int NumberOfGuests { get; set; }
        public string? TrnxReference { get; set; }
        public string? PaymentChannel { get; set; }
        public bool PaymentStatus { get; set; }
        public string? Comment { get; set; }
        public string? TicketNum { get; set; }
        public DateTime ReservationStartDate { get; set; }
        public DateTime ReservationEndDate { get; set; }
        public int? NoOfDays { get; set; }
        public bool TicketClosed { get; set; }
        public double? TotalCost { get; set; }
        public DateTime UpdateddAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid StaffId { get; set; }
        public string? StaffName { get; set; }
        public string? PurchaseChannel { get; set; }
        public string? UserType { get; set; } // e.g., "Registerd", "Not Registered"

    }

    public class AddCustomerBookedReservation
    {
        public Guid UserId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhoneNumber { get; set; }
        public string? CustomerEmail { get; set; }
        public int? NumberOfGuests { get; set; }
        public string? TrnxReference { get; set; }
        public string? PaymentChannel { get; set; }
        public bool? PaymentStatus { get; set; }
        public string? Comment { get; set; }
        public bool? TicketClosed { get; set; }
        public string? ReservationStartDate { get; set; }
        public string? ReservationEndDate { get; set; }
        public int? NoOfDays { get; set; }
        public Guid StaffId { get; set; }
        public string? StaffName { get; set; }
        public string? PurchaseChannel { get; set; }
        public string? UserType { get; set; } // e.g., "Registerd", "Not Registered"
    }
}
