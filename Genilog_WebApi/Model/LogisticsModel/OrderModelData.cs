namespace Genilog_WebApi.Model.LogisticsModel
{
    public enum OrderStatusData { Open, Accepted, Rejected, Picked, Ongoing, Delivered, Completed, Closed }
    public class OrderModelData
    {
        public Guid Id { get; set; }
        public string? TrackingNum { get; set; }
        public string? ItemName { get; set; }
        public string? ItemDescription { get; set; }
        public string? ItemModelNumber { get; set; }
        public double ItemCost { get; set; }
        public int ItemQuantity { get; set; }
        public string? PackageType { get; set; }
        public string? ExpectedDeliveryTime { get; set; }
        public string? OrderStatus { get; set; }
        // Sender Info
        public Guid UserId { get; set; }
        public string? SenderName { get; set; }
        public string? SenderPhoneNo { get; set; }
        public string? SenderEmail { get; set; }
        public string? SenderAddress { get; set; }
        public string? SenderState { get; set; }
        public string? SenderLocality { get; set; }
        public string? SenderPostalCode { get; set; }
        public double SenderLatitude { get; set; }
        public double SenderLongitude { get; set; }
        // Receiver Info
        public string? RecieverName { get; set; }
        public string? RecieverPhoneNo { get; set; }
        public string? RecieverEmail { get; set; }
        public string? RecieverAddress { get; set; }
        public string? RecieverState { get; set; }
        public string? RecieverLocality { get; set; }
        public string? RecieverPostalCode { get; set; }
        public double RecieverLatitude { get; set; }
        public double RecieverLongitude { get; set; }
        // Company Info
        public Guid CompanyId { get; set; }
        
        public Guid RiderId { get; set; }
        public string? CompanyName { get; set; }
        public string? RiderName { get; set; }
        public string? CompanyPhoneNo { get; set; }
        public string? CompanyEmail { get; set; }
        public string? CompanyAddress { get; set; }
        public double CurrentLatitude { get; set; }
        public double CurrentLongitude { get; set; }
        public string? CurrentLocation { get; set; }
        public string? ConfirmationImage { get; set; }
        // Payment Details
        public double ShippingCost { get; set; }
        public string? TrnxReference { get; set; }
        public string? PaymentChannel { get; set; }
        public bool PaymentStatus { get; set; }
        public string? QRCode { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<PackageImageList>? PackageImageLists { get; set; }
    }
    public class PackageImageList
    {
        public Guid Id { get; set; }
        public string? ImageUrlFile { get; set; }
        public OrderModelData? OrderModelDatas { get; set; }
        public Guid OrderModelDataId { get; set; }
    }
    public class OrderModelDataDto
    {
        public Guid Id { get; set; }
        public string? TrackingNum { get; set; }
        public string? ItemName { get; set; }
        public string? ItemDescription { get; set; }
        public string? ItemModelNumber { get; set; }
        public double ItemCost { get; set; }
        public int ItemQuantity { get; set; }
        public string? PackageType { get; set; }
        public string? ExpectedDeliveryTime { get; set; }
        public string? OrderStatus { get; set; }
        // Sender Info
        public Guid UserId { get; set; }
        public string? SenderName { get; set; }
        public string? SenderPhoneNo { get; set; }
        public string? SenderEmail { get; set; }
        public string? SenderAddress { get; set; }
        public string? SenderState { get; set; }
        public string? SenderLocality { get; set; }
        public string? SenderPostalCode { get; set; }
        public double SenderLatitude { get; set; }
        public double SenderLongitude { get; set; }
        // Receiver Info
        public string? RecieverName { get; set; }
        public string? RecieverPhoneNo { get; set; }
        public string? RecieverEmail { get; set; }
        public string? RecieverAddress { get; set; }
        public string? RecieverState { get; set; }
        public string? RecieverLocality { get; set; }
        public string? RecieverPostalCode { get; set; }
        public double RecieverLatitude { get; set; }
        public double RecieverLongitude { get; set; }
        // Company Info
        public Guid CompanyId { get; set; }
        
        public Guid RiderId { get; set; }
        public string? RiderName { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyPhoneNo { get; set; }
        public string? CompanyEmail { get; set; }
        public string? CompanyAddress { get; set; }
        public double CurrentLatitude { get; set; }
        public double CurrentLongitude { get; set; }
        public string? CurrentLocation { get; set; }
        public string? ConfirmationImage { get; set; }
        // Payment Details
        public double ShippingCost { get; set; }
        public string? TrnxReference { get; set; }
        public string? PaymentChannel { get; set; }
        public bool PaymentStatus { get; set; }
        public string? QRCode { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<PackageImageListDto>? PackageImageLists { get; set; }
    }
    public class PackageImageListDto
    {
        public Guid Id { get; set; }
        public string? ImageUrlFile { get; set; }
        public Guid OrderModelDataId { get; set; }
    }
    public class AddOrder
    {
        public string? ItemName { get; set; }
        public string? ItemDescription { get; set; }
        public string? ItemModelNumber { get; set; }
        public double ItemCost { get; set; }
        public int ItemQuantity { get; set; }
        public string? PackageType { get; set; }
        // Sender
        public string? SenderAddress { get; set; }
        public string? SenderState { get; set; }
        public string? SenderLocality { get; set; }
        public string? SenderPostalCode { get; set; }
        public double SenderLatitude { get; set; }
        public double SenderLongitude { get; set; }
        // Receiver Info
        public string? RecieverName { get; set; }
        public string? RecieverPhoneNo { get; set; }
        public string? RecieverEmail { get; set; }
        public string? RecieverAddress { get; set; }
        public string? RecieverState { get; set; }
        public string? RecieverLocality { get; set; }
        public string? RecieverPostalCode { get; set; }
        public double RecieverLatitude { get; set; }
        public double RecieverLongitude { get; set; }

    }

    public class UpdateOrder
    {
        public string? ExpectedDeliveryTime { get; set; }
        public string? OrderStatus { get; set; }
        public string? ConfirmationImage { get; set; }
        public double? ShippingCost { get; set; }
        public string? TrnxReference { get; set; }
        public string? PaymentChannel { get; set; }
        public bool? PaymentStatus { get; set; }
        public string? Comment { get; set; }
        public double? CurrentLatitude { get; set; }
        public double? CurrentLongitude { get; set; }
        public string? CurrentLocation { get; set; }
    }


    public class UpdatesPurchaseImages
    {
        public List<string>? ImageURL { get; set; }
    }
}
