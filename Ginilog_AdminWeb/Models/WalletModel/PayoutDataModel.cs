using Newtonsoft.Json;

namespace Ginilog_AdminWeb.Models.WalletModel
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


    public class PaymentRequest
    {
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public decimal Amount { get; set; }
    }

    // Model for Paystack Response
    public class PaystackResponse
    {
        public bool Status { get; set; }
        public string? Message { get; set; }
        public PaystackData? Data { get; set; }
    }

    public class PaystackData
    {
        public string? AuthorizationUrl { get; set; }
        public string? AccessCode { get; set; }
        public string? Reference { get; set; }
    }

    public class PaymentSucessData
    {
        public bool Status { get; set; }
        public string? Message { get; set; }
        public string? Reference { get; set; }
        public string? AccessCode { get; set; }
        public string? PaymentStatus { get; set; }
    }

    // Flutterwave
    public partial class FlutterwaveResponse
    {
        public string? Status { get; set; }
        public string? Message { get; set; }
        public Data? Data { get; set; }
    }

    public partial class Data
    {
        public string? Link { get; set; }
    }

}
