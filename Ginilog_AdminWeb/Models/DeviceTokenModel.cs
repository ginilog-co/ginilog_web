namespace BMG_Admin_Panel.Models
{
    public class DeviceTokenModel
    {
        public Guid Id { get; set; }
        public string? DeviceTokenId { get; set; }
        public Guid UserId { get; set; }
        public string? UserType { get; set; }
    }
}
