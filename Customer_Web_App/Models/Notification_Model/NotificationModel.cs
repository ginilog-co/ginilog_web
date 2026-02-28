namespace Customer_Web_App.Models.Notification_Model
{

    public class NotificationModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; }
        public string? NotificationType { get; set; }
        public bool? IsRead { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class AddNotification
    {
        public string? Title { get; set; }
        public string? Body { get; set; }
        public string? DeviceToken { get; set; }
        public bool? IsRead { get; set; }
        public string? NotificationType { get; set; }
    }
}
