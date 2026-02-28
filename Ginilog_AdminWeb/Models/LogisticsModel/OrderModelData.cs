namespace Ginilog_AdminWeb.Models.LogisticsModel
{
    public enum OrderStatusData { Open, Picked, Booked, InTransit, Delivered, Cancelled }
    public class OrderModelData
    {
        public Guid Id { get; set; }
        public string? TrackingNum { get; set; }
        public string? ItemName { get; set; }
        public string? ItemDescription { get; set; }
        public string? ItemModelNumber { get; set; }
        public double ItemCost { get; set; }
        public double ItemWeight { get; set; }
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
        public string? SenderCountry { get; set; }
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
        public string? RecieverCountry { get; set; }
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
        public double VatCost { get; set; }
        public string? TrnxReference { get; set; }
        public string? PaymentChannel { get; set; }
        public bool PaymentStatus { get; set; }
        public string? QRCode { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<string>? PackageImageLists { get; set; }
        public string? RiderType { get; set; }
        public string? ShippingType { get; set; }
        public List<OrderDeliveryFlow>? OrderDeliveryFlows { get; set; }
        public Guid StaffId { get; set; }
        public string? StaffName { get; set; }
        public string? PurchaseChannel { get; set; }
        public string? UserType { get; set; } // e.g., "Registerd", "Not Registered"
    }

    public class OrderDeliveryFlow
    {
        public Guid Id { get; set; }
        public string? OrderStatus { get; set; }
        public double CurrentLatitude { get; set; }
        public double CurrentLongitude { get; set; }
        public string? CurrentLocation { get; set; }
        public Guid OrderModelDataId { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class AddOrder
    {
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
        public string? ExpectedDeliveryTime { get; set; }
        public double ShippingCost { get; set; }
        public double VatCost { get; set; }
        public string? ItemName { get; set; }
        public string? ItemDescription { get; set; }
        public string? ItemModelNumber { get; set; }
        public double ItemCost { get; set; }
        public int ItemQuantity { get; set; }
        public double ItemWeight { get; set; }
        public string? PackageType { get; set; }
        // Sender
        public string? SenderName { get; set; }
        public string? SenderPhoneNo { get; set; }
        public string? SenderEmail { get; set; }
        public string? SenderAddress { get; set; }
      //  public string? SenderState { get; set; }
      //  public string? SenderCountry { get; set; }
       // public string? SenderLocality { get; set; }
       // public string? SenderPostalCode { get; set; }
      //  public double SenderLatitude { get; set; }
      //  public double SenderLongitude { get; set; }
        // Receiver Info
        public string? RecieverName { get; set; }
        public string? RecieverPhoneNo { get; set; }
        public string? RecieverEmail { get; set; }
        public string? RecieverAddress { get; set; }
       // public string? RecieverState { get; set; }
      //  public string? RecieverCountry { get; set; }
      //  public string? RecieverLocality { get; set; }
       // public string? RecieverPostalCode { get; set; }
      //  public double RecieverLatitude { get; set; }
      //  public double RecieverLongitude { get; set; }
        public List<string>? PackageImageLists { get; set; }
        public string? RiderType { get; set; }
        public string? ShippingType { get; set; }
        public List<IFormFile>? ImageList { get; set; }
        public string? PaymentChannel { get; set; }

    }

    public class AddMainOrder
    {
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
        public string? ExpectedDeliveryTime { get; set; }
        public double ShippingCost { get; set; }
        public double VatCost { get; set; }
        public string? ItemName { get; set; }
        public string? ItemDescription { get; set; }
        public string? ItemModelNumber { get; set; }
        public double ItemCost { get; set; }
        public int ItemQuantity { get; set; }
        public double ItemWeight { get; set; }
        public string? PackageType { get; set; }
        // Sender
        public string? SenderName { get; set; }
        public string? SenderPhoneNo { get; set; }
        public string? SenderEmail { get; set; }
        public string? SenderAddress { get; set; }
        public string? SenderState { get; set; }
        public string? SenderCountry { get; set; }
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
        public string? RecieverCountry { get; set; }
        public string? RecieverLocality { get; set; }
        public string? RecieverPostalCode { get; set; }
        public double RecieverLatitude { get; set; }
        public double RecieverLongitude { get; set; }
        public List<string>? PackageImageLists { get; set; }
        public string? RiderType { get; set; }
        public string? ShippingType { get; set; }
        public List<IFormFile>? ImageList { get; set; }
        public string? PaymentChannel { get; set; }
        public Guid StaffId { get; set; }
        public string? StaffName { get; set; }
        public string? PurchaseChannel { get; set; }
        public string? UserType { get; set; } // e.g., "Registerd", "Not Registered"

    }

    public class UpdateOrder
    {
        public string? ExpectedDeliveryTime { get; set; }
        public string? OrderStatus { get; set; }
        public string? ConfirmationImage { get; set; }
        public double? ShippingCost { get; set; }
        public double? VatCost { get; set; }
        public string? TrnxReference { get; set; }
        public string? PaymentChannel { get; set; }
        public bool? PaymentStatus { get; set; }
        public string? Comment { get; set; }
        public double? CurrentLatitude { get; set; }
        public double? CurrentLongitude { get; set; }
        public string? CurrentLocation { get; set; }
    }

    public class UpdateShippingCost
    {
        public Guid Id { get; set; }
        public double? ShippingCost { get; set; }
        public double? VatCost { get; set; }
    }

    public class UpdateExpectedDeliveryTime
    {
        public Guid Id { get; set; }
        public string? ExpectedDeliveryTime { get; set; }
    }

    public class UpdateOrderStatus
    {
        public Guid Id { get; set; }
        public string? OrderStatus { get; set; }
    }

    public class AllOrdersDataModel
    {
        public List<OrderModelData>? OrderModelData { get; set; }
        public OrderStatistics? OrderStatistics { get; set; }
    } 
    public class OrdersDetailsDataModel
    {
        public OrderModelData? OrderModelData { get; set; }
       public UpdateShippingCost? UpdateShippingCost { get; set; }
       public UpdateExpectedDeliveryTime? UpdateExpectedDeliveryTime { get; set; }
       public UpdateOrderStatus? UpdateOrderStatus { get; set; }
    }

}
