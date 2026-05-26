namespace Genilog_WebApi.Model
{
    public class FilterLocationData
    {
        public string? UserId { get; set; }
        public string? State { get; set; }
        public string? Locality { get; set; }
        public string? AnyItem { get; set; }
        public string? FilterTypes { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
        public class FilterData
    {
        public string? AnyItem { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }

    public class FilterByPageData
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }

    public class FilterAllData
    {
        public string? State { get; set; }
        public string? Locality { get; set; }
        public int? Days { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? UserId { get; set; }
        public string? AnyItem { get; set; }
    }
}
