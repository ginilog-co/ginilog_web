
namespace Genilog_WebApi.Model.WalletModel
{
    public class PayoutDataModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Name { get; set; }
        public double Amount { get; set; }
        public bool PayOutStatus { get; set; }
        public string? PaymentTo { get; set; }
        public string? Remark { get; set; }
        public string? TransactionReference { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime PayOutDateAt { get; set; }
        public string? Address { get; set; }
        public string? PostCodes { get; set; }
        public string? Locality { get; set; }
        public string? State { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
   
    public class PayoutDataModelDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Name { get; set; }
        public double Amount { get; set; }
        public bool PayOutStatus { get; set; }
        public string? PaymentTo { get; set; }
        public string? Remark { get; set; }
        public string? TransactionReference { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime PayOutDateAt { get; set; }
        public string? Address { get; set; }
        public string? PostCodes { get; set; }
        public string? Locality { get; set; }
        public string? State { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
    public class AddPayout
    {
        public string? TransactionReference { get; set; }
        public double Amount { get; set; }
    }
}
