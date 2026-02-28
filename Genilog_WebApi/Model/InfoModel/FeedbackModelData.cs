using Google.Cloud.Firestore;

namespace Genilog_WebApi.Model.InfoModel
{
    public class FeedbackModelData
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Feedback { get; set; }
        public string? PhoneNo { get; set; }
        public string? CompanyName { get; set; }
        public string? DatePublished { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class FeedbackModelDataDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Feedback { get; set; }
        public string? PhoneNo { get; set; }
        public string? CompanyName { get; set; }
        public string? DatePublished { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class SearchFeedback
    {
        public string? Search { get; set; }
    }
}
