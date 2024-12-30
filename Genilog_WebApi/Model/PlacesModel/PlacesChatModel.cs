namespace Genilog_WebApi.Model.PlacesModel
{
    public class PlacesChatModel
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
    public class PlacesChatModelDto
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

    // Hotel Chat Model
    public class HotelChatModel
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string? GroupChatId { get; set; }
        public string? Message { get; set; }
        public string? MessageType { get; set; }
        public string? ItemImageURL { get; set; }
        public bool IsRead { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
    public class HotelChatModelDto
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string? GroupChatId { get; set; }
        public string? Message { get; set; }
        public string? MessageType { get; set; }
        public string? ItemImageURL { get; set; }
        public bool IsRead { get; set; }
        public string? SenderUserName { get; set; }
        public string? SenderProfileImage { get; set; }
        public string? ReceiverUserName { get; set; }
        public string? ReceiverProfileImage { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
