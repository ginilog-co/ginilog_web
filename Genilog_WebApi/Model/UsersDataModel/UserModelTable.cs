using Genilog_WebApi.Model.AuthModel;

namespace Genilog_WebApi.Model.UsersDataModel
{
    public class UsersDataModelTable
    {
        public Guid Id { get; set; }
        public string? FirstName{get;set;}
        public string? LastName { get; set; }
        public string? Email{get;set;}
        public string? PhoneNo{get;set;}
        public string? ProfilePicture{get;set;}
        public string? Sex{get;set;}
        public string? ReferralCode { get; set; }
        public bool UserStatus { get; set; }
        public string? Address { get; set; }
        public string? PostCodes { get; set; }
        public string? Locality { get; set; }
        public string? State { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<DeliveryAddress>? DeliveryAddresses { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime? LastSeenAt { get; set; }
    }



    public class DeliveryAddress
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNo { get; set; }
        public string? Address { get; set; }
        public string? AddressPostCodes { get; set; }
        public string? Locality { get; set; }
        public string? State { get; set; }
        public string? HouseNo { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public UsersDataModelTable? UsersDataModelTable { get; set; }
        public Guid UsersDataModelTableId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
    public class UsersDataModelTableDto
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
        public List<DeliveryAddressDto>? DeliveryAddresses { get; set; }
        public List<DeviceTokenModel>? DeviceTokenModels { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime? LastSeenAt { get; set; }
    }

    public class DeliveryAddressDto
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


}
