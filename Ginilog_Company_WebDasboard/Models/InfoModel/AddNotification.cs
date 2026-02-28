namespace Ginilog_Company_WebDasboard.Models.InfoModel
{

    public class AddNotification
    {
        public string? Title { get; set; }
        public string? Body { get; set; }
        public string? DeviceToken { get; set; }
        public string? NotificationType { get; set; }
        public Guid UserId { get; set; }
    }
}
