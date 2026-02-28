namespace Ginilog_AdminWeb.Models.InfoModel
{
    public class FeedbackModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? UserGoogleUid { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Feedback { get; set; }
        public string? PhoneNo { get; set; }
        public string? DatePublished { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
