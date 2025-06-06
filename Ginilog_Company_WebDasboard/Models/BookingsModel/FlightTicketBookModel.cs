namespace Ginilog_Company_WebDasboard.Models.BookingsModel
{
    public class FlightTicketBookModel
    {
        public Guid Id { get; set; }
        public Guid AirlineId { get; set; }
        public Guid AdminId { get; set; }
        public string? AirlineName { get; set; }
        public string? DepartureAirpot { get; set; }
        public string? ReturnAirpot { get; set; }
        public string? OperatedBy { get; set; }
        public string? FlightSpeed { get; set; }
        public string? TicketNum { get; set; }
        public string? DapatureTime { get; set; }
        public string? AvailabeTimeInterval { get; set; }
        public string? Dapature { get; set; }
        public string? Destination { get; set; }
        public string? TicketType { get; set; }
        public List<string>? StopPlaces { get; set; }
        public int? BigLuggageKg { get; set; }
        public int? SmallLuggageKg { get; set; }
        public int? Stops { get; set; }
        public bool? Available { get; set; }
        public bool? IsReturn { get; set; }
        public double? TicketPrice { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class FlightTicketBookModelDto
    {
        public Guid Id { get; set; }
        public Guid AirlineId { get; set; }
        public Guid AdminId { get; set; }
        public string? AirlineName { get; set; }
        public string? OperatedBy { get; set; }
        public string? FlightSpeed { get; set; }
        public string? TicketNum { get; set; }
        public string? DapatureTime { get; set; }
        public string? AvailabeTimeInterval { get; set; }
        public string? Dapature { get; set; }
        public string? Destination { get; set; }
        public string? TicketType { get; set; }
        public List<string>? StopPlaces { get; set; }
        public int? BigLuggageKg { get; set; }
        public int? SmallLuggageKg { get; set; }
        public int? Stops { get; set; }
        public bool? Available { get; set; }
        public bool? IsReturn { get; set; }
        public double? TicketPrice { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class AddFlightTicket
    {
        public Guid AirlineId { get; set; }
        public string? OperatedBy { get; set; }
        public string? FlightSpeed { get; set; }
        public string? DapatureTime { get; set; }
        public string? AvailabeTimeInterval { get; set; }
        public string? Dapature { get; set; }
        public string? Destination { get; set; }
        public string? TicketType { get; set; }
        public List<string>? StopPlaces { get; set; }
        public int? BigLuggageKg { get; set; }
        public int? SmallLuggageKg { get; set; }
        public int? Stops { get; set; }
        public bool? Available { get; set; }
        public bool? IsReturn { get; set; }
        public double? TicketPrice { get; set; }
    }

}
