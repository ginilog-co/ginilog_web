namespace Genilog_WebApi.Model.LogisticsModel
{
    public class RidersChatModelData
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string? GroupChatId { get; set; }
        public string? Message { get; set; }
        public string? MessageType { get; set; }
        public bool IsRead { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
    public class RidersChatModelDataDto
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string? GroupChatId { get; set; }
        public string? Message { get; set; }
        public string? MessageType { get; set; }
        public bool IsRead { get; set; }
        public string? SenderUserName { get; set; }
        public string? SenderProfileImage { get; set; }
        public string? ReceiverUserName { get; set; }
        public string? ReceiverProfileImage { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class AddChatMessage
    {

        public string? Message { get; set; }
        public string? MessageType { get; set; }
    }
    public class UpdateChatMessage
    {
        public string? Message { get; set; }
    }
    public class UpdateChatIsReadMessage
    {
        public bool IsRead { get; set; }
    }
}
