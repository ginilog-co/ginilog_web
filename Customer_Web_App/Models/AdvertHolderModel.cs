namespace Customer_Web_App.Models
{
    public class AdvertHolderModel
    {
        public Guid Id { get; set; }
        public Guid AdminId { get; set; }
        public Guid AdvertItemId { get; set; }
        public string? AdvertImage { get; set; }
        public string? AdvertName { get; set; }
        public string? AdvertType { get; set; }
        public string? AdvertItemDescription { get; set; }
        public double AdvertItemCost { get; set; }
        public string? TransRef { get; set; } // Assuming this is a different cost for some reason
        public bool TransStatus { get; set; }
        public int AdvertDays4 { get; set; }
        public DateTime ExpiredAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AddMainAdvert
    {
        public Guid AdvertItemId { get; set; }
        public string? AdvertImage { get; set; }
        public string? AdvertName { get; set; }
        public string? AdvertType { get; set; }
        public string? AdvertItemDescription { get; set; }
        public int? AdvertDays4 { get; set; }
    }
    public class AddAdvert
    {
        public IFormFile? AdvertImage { get; set; }
        public string? AdvertName { get; set; }
        public string? AdvertType { get; set; }
        public string? AdvertItemDescription { get; set; }
        public int? AdvertDays4 { get; set; }
    }

    public class UpdateAdvert
    {
        public Guid Id { get; set; }
        public IFormFile? AdvertImage { get; set; }
        public string? AdvertName { get; set; }
        public string? AdvertItemDescription { get; set; }
        public int? AdvertDays4 { get; set; }
    }

    public class MainAdvertModel
    {
        public AdvertHolderModel? AdvertHolderModel { get; set; }
        public UpdateAdvert? AddAdvert { get; set; }
    }
}
