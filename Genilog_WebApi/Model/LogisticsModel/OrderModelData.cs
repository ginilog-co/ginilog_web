using Newtonsoft.Json;

namespace Genilog_WebApi.Model.LogisticsModel
{
    public enum OrderStatusData { Open, Accepted, Rejected, Picked, Ongoing, Delivered, Completed, Closed }
    public class OrderModelData
    {
        public Guid Id { get; set; }
        public Guid AdminId { get; set; }
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
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<string>? PackageImageLists { get; set; }
        public string? RiderType { get; set; }
        public string? ShippingType { get; set; }
        public List<OrderDeliveryFlow>? OrderDeliveryFlows { get; set; }
        public Guid StaffId { get; set; }
        public string? StaffName { get; set; }
        public string? PurchaseChannel { get;set; }
        public string? UserType { get; set; } // e.g., "Registerd", "Not Registered"
    }

    public class OrderDeliveryFlow
    {
        public Guid Id { get; set; }
        public string? OrderStatus { get; set; }
        public double CurrentLatitude { get; set; }
        public double CurrentLongitude { get; set; }
        public string? CurrentLocation { get; set; }
       public OrderModelData? OrderModelDatas { get; set; }
        public Guid OrderModelDataId { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
 

    public class OrderModelDataDto
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("adminId")]
        public Guid AdminId { get; set; }

        [JsonProperty("trackingNum")]
        public string? TrackingNum { get; set; }

        [JsonProperty("itemName")]
        public string? ItemName { get; set; }

        [JsonProperty("itemDescription")]
        public string? ItemDescription { get; set; }

        [JsonProperty("itemModelNumber")]
        public string? ItemModelNumber { get; set; }

        [JsonProperty("itemCost")]
        public double ItemCost { get; set; }

        [JsonProperty("itemWeight")]
        public double ItemWeight { get; set; }

        [JsonProperty("itemQuantity")]
        public int ItemQuantity { get; set; }

        [JsonProperty("packageType")]
        public string? PackageType { get; set; }

        [JsonProperty("expectedDeliveryTime")]
        public string? ExpectedDeliveryTime { get; set; }

        [JsonProperty("orderStatus")]
        public string? OrderStatus { get; set; }

        // Sender Info
        [JsonProperty("userId")]
        public Guid UserId { get; set; }

        [JsonProperty("senderName")]
        public string? SenderName { get; set; }

        [JsonProperty("senderPhoneNo")]
        public string? SenderPhoneNo { get; set; }

        [JsonProperty("senderEmail")]
        public string? SenderEmail { get; set; }

        [JsonProperty("senderAddress")]
        public string? SenderAddress { get; set; }

        [JsonProperty("senderState")]
        public string? SenderState { get; set; }

        [JsonProperty("senderCountry")]
        public string? SenderCountry { get; set; }

        [JsonProperty("senderLocality")]
        public string? SenderLocality { get; set; }

        [JsonProperty("senderPostalCode")]
        public string? SenderPostalCode { get; set; }

        [JsonProperty("senderLatitude")]
        public double SenderLatitude { get; set; }

        [JsonProperty("senderLongitude")]
        public double SenderLongitude { get; set; }

        // Receiver Info
        [JsonProperty("recieverName")]
        public string? RecieverName { get; set; }

        [JsonProperty("recieverPhoneNo")]
        public string? RecieverPhoneNo { get; set; }

        [JsonProperty("recieverEmail")]
        public string? RecieverEmail { get; set; }

        [JsonProperty("recieverAddress")]
        public string? RecieverAddress { get; set; }

        [JsonProperty("recieverState")]
        public string? RecieverState { get; set; }

        [JsonProperty("recieverCountry")]
        public string? RecieverCountry { get; set; }

        [JsonProperty("recieverLocality")]
        public string? RecieverLocality { get; set; }

        [JsonProperty("recieverPostalCode")]
        public string? RecieverPostalCode { get; set; }

        [JsonProperty("recieverLatitude")]
        public double RecieverLatitude { get; set; }

        [JsonProperty("recieverLongitude")]
        public double RecieverLongitude { get; set; }

        // Company Info
        [JsonProperty("companyId")]
        public Guid CompanyId { get; set; }

        [JsonProperty("riderId")]
        public Guid RiderId { get; set; }

        [JsonProperty("riderName")]
        public string? RiderName { get; set; }

        [JsonProperty("companyName")]
        public string? CompanyName { get; set; }

        [JsonProperty("companyPhoneNo")]
        public string? CompanyPhoneNo { get; set; }

        [JsonProperty("companyEmail")]
        public string? CompanyEmail { get; set; }

        [JsonProperty("companyAddress")]
        public string? CompanyAddress { get; set; }

        [JsonProperty("currentLatitude")]
        public double CurrentLatitude { get; set; }

        [JsonProperty("currentLongitude")]
        public double CurrentLongitude { get; set; }

        [JsonProperty("currentLocation")]
        public string? CurrentLocation { get; set; }

        [JsonProperty("confirmationImage")]
        public string? ConfirmationImage { get; set; }

        // Payment Details
        [JsonProperty("shippingCost")]
        public double ShippingCost { get; set; }

        [JsonProperty("vatCost")]
        public double VatCost { get; set; }

        [JsonProperty("trnxReference")]
        public string? TrnxReference { get; set; }

        [JsonProperty("paymentChannel")]
        public string? PaymentChannel { get; set; }

        [JsonProperty("paymentStatus")]
        public bool PaymentStatus { get; set; }

        [JsonProperty("qrCode")]
        public string? QRCode { get; set; }

        [JsonProperty("comment")]
        public string? Comment { get; set; }

        [JsonProperty("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [JsonProperty("packageImageLists")]
        public List<string>? PackageImageLists { get; set; }

        [JsonProperty("riderType")]
        public string? RiderType { get; set; }

        [JsonProperty("shippingType")]
        public string? ShippingType { get; set; }

        [JsonProperty("orderDeliveryFlows")]
        public List<OrderDeliveryFlowDto>? OrderDeliveryFlows { get; set; }

        [JsonProperty("staffId")]
        public Guid StaffId { get; set; }

        [JsonProperty("staffName")]
        public string? StaffName { get; set; }

        [JsonProperty("purchaseChannel")]
        public string? PurchaseChannel { get; set; }

        [JsonProperty("userType")]
        public string? UserType { get; set; }
    }


    public class OrderDeliveryFlowDto
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("orderStatus")]
        public string? OrderStatus { get; set; }

        [JsonProperty("currentLatitude")]
        public double CurrentLatitude { get; set; }

        [JsonProperty("currentLongitude")]
        public double CurrentLongitude { get; set; }

        [JsonProperty("currentLocation")]
        public string? CurrentLocation { get; set; }

        [JsonProperty("orderModelDataId")]
        public Guid OrderModelDataId { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
    }


    public class AddOrder
    {
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
        public string? SenderCountry { get; set; }
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
        public string? RecieverCountry { get; set; }
        public string? RecieverState { get; set; }
        public string? RecieverLocality { get; set; }
        public string? RecieverPostalCode { get; set; }
        public double RecieverLatitude { get; set; }
        public double RecieverLongitude { get; set; }
        public List<string>? PackageImageLists { get; set; }
        public string? RiderType { get; set; }
        public string? ShippingType { get; set; }
        public Guid StaffId { get; set; }
        public string? StaffName { get; set; }
        public string? PurchaseChannel { get; set; }
        public string? UserType { get; set; } // e.g., "Registerd", "Not Registered"

    }

    public class AddModelOrder
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
        public string? SenderCountry { get; set; }
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
        public string? RecieverCountry { get; set; }
        public string? RecieverState { get; set; }
        public string? RecieverLocality { get; set; }
        public string? RecieverPostalCode { get; set; }
        public double RecieverLatitude { get; set; }
        public double RecieverLongitude { get; set; }
        public List<string>? PackageImageLists { get; set; }
        public string? RiderType { get; set; }
        public string? ShippingType { get; set; }
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
}
