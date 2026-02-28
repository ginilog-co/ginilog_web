using BMG_Admin_Panel.Models;
using Ginilog_Company_WebDasboard.Models.BookingsModel;
using Ginilog_Company_WebDasboard.Models.InfoModel;
using Ginilog_Company_WebDasboard.Models.LogisticsModel;

namespace Ginilog_Company_WebDasboard.Models.UsersDataModel
{

    public class UserModelTable
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNo { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Sex { get; set; }
        public string? ReferralCode { get; set; }
        public bool UserStatus { get; set; }
        public string? Address { get; set; }
        public string? PostCodes { get; set; }
        public string? Locality { get; set; }
        public string? State { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double MoneyBoxBalance { get; set; }
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public List<DeliveryAddress>? DeliveryAddresses { get; set; }
        public List<DeviceTokenModel>? DeviceTokenModels { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime? LastSeenAt { get; set; }
    }


    public class DeliveryAddress
    {
        public Guid Id { get; set; }
        public string? Address { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNo { get; set; }
        public string? AddressPostCodes { get; set; }
        public string? Locality { get; set; }
        public string? State { get; set; }
        public string? HouseNo { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Guid UsersDataModelTableId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class AddDeliveryAddress
    {
        public string? Address { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNo { get; set; }
        public string? AddressPostCodes { get; set; }
        public string? HouseNo { get; set; }
        public string? Locality { get; set; }
        public string? State { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }

    public class UserDetailsModel
    {
        public UserModelTable? UserModelTable { get; set; }
        public AddNotification? AddNotification { get; set; }
        public SendMailModel? SendMailModel { get; set; }
        public List<CustomerBookedReservation>? CustomerBookedReservations { get; set; }
        public List<OrderModelData>? OrderModelDatas { get; set; }
    }

}
