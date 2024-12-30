
namespace Genilog_WebApi.Model.PlacesModel
{
    public class AllTimeSchedule
    {
    }
    public class PlacesMondayModel
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public PlacesDataModel? PlacesDataModels { get; set; }
        public Guid PlacesDataModelId { get; set; }
    }
    public class PlacesTuesdayModel
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public PlacesDataModel? PlacesDataModels { get; set; }
        public Guid PlacesDataModelId { get; set; }
    }
    public class PlacesWednesdayModel
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public PlacesDataModel? PlacesDataModels { get; set; }
        public Guid PlacesDataModelId { get; set; }
    }
    public class PlacesThursdayModel
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public PlacesDataModel? PlacesDataModels { get; set; }
        public Guid PlacesDataModelId { get; set; }
    }
    public class PlacesFridayModel
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public PlacesDataModel? PlacesDataModels { get; set; }
        public Guid PlacesDataModelId { get; set; }
    }
    public class PlacesSaturdayModel
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public PlacesDataModel? PlacesDataModels { get; set; }
        public Guid PlacesDataModelId { get; set; }
    }
    public class PlacesSundayModel
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public PlacesDataModel? PlacesDataModels { get; set; }
        public Guid PlacesDataModelId { get; set; }
    }
    // dtos
    public class PlacesMondayModelDto
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public Guid PlacesDataModelId { get; set; }
    }
    public class PlacesTuesdayModelDto
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public Guid PlacesDataModelId { get; set; }
    }
    public class PlacesWednesdayModelDto
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public Guid PlacesDataModelId { get; set; }
    }
    public class PlacesThursdayModelDto
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public Guid PlacesDataModelId { get; set; }
    }
    public class PlacesFridayModelDto
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public Guid PlacesDataModelId { get; set; }
    }
    public class PlacesSaturdayModelDto
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public Guid PlacesDataModelId { get; set; }
    }
    public class PlacesSundayModelDto
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public Guid PlacesDataModelId { get; set; }
    }

    // Hotel Time Schedule
    public class HotelMondayModel
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public HotelDataModel? HotelDataModels { get; set; }
        public Guid HotelDataModelId { get; set; }
    }
    public class HotelTuesdayModel
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public HotelDataModel? HotelDataModels { get; set; }
        public Guid HotelDataModelId { get; set; }
    }
    public class HotelWednesdayModel
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public HotelDataModel? HotelDataModels { get; set; }
        public Guid HotelDataModelId { get; set; }
    }
    public class HotelThursdayModel
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public HotelDataModel? HotelDataModels { get; set; }
        public Guid HotelDataModelId { get; set; }
    }
    public class HotelFridayModel
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public HotelDataModel? HotelDataModels { get; set; }
        public Guid HotelDataModelId { get; set; }
    }
    public class HotelSaturdayModel
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public HotelDataModel? HotelDataModels { get; set; }
        public Guid HotelDataModelId { get; set; }
    }
    public class HotelSundayModel
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public HotelDataModel? HotelDataModels { get; set; }
        public Guid HotelDataModelId { get; set; }
    }
    // dtos
    public class HotelMondayModelDto
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public Guid HotelDataModelId { get; set; }
    }
    public class HotelTuesdayModelDto
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public Guid HotelDataModelId { get; set; }
    }
    public class HotelWednesdayModelDto
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public Guid HotelDataModelId { get; set; }
    }
    public class HotelThursdayModelDto
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public Guid PlacesDataModelId { get; set; }
    }
    public class HotelFridayModelDto
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public Guid HotelDataModelId { get; set; }
    }
    public class HotelSaturdayModelDto
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public Guid HotelDataModelId { get; set; }
    }
    public class HotelSundayModelDto
    {
        public Guid Id { get; set; }
        public string? HourStart { get; set; }
        public string? HourEnd { get; set; }
        public bool? IsClosed { get; set; }
        public Guid HotelDataModelId { get; set; }
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
        public string? Start { get; set; }
        public string? End { get; set; }
        public bool IsClosed { get; set; }
    }
}
