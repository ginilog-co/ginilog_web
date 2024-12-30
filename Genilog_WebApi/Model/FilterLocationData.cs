namespace Genilog_WebApi.Model
{
    public class FilterLocationData
    {
        public string? State { get; set; }
        public string? Locality { get; set; }
        public string? AnyItem { get; set; }
    }

    public class FilterAllData
    {
        public string? State { get; set; }
        public string? Locality { get; set; }
        public int? Days { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? UserId { get; set; }
    }
}
