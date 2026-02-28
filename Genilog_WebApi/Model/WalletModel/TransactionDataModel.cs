using Newtonsoft.Json;

namespace Genilog_WebApi.Model.WalletModel
{
    public class TransactionDataModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid AdminId { get; set; }
        public string? Title { get; set; }
        public string? TransactionType { get; set; }
        public string? Reason { get; set; }
        public string? Amount { get; set; }
        public string? PhoneNo { get; set; }
        public string? TrnxRef { get; set; }
        public bool TrnxStatus { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class TransactionDataModelDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid AdminId { get; set; }
        public string? Title { get; set; }
        public string? TransactionType { get; set; }
        public string? Reason { get; set; }
        public string? Amount { get; set; }
        public string? PhoneNo { get; set; }
        public string? TrnxRef { get; set; }
        public bool TrnxStatus { get; set; }
        public DateTime CreatedAt { get; set; }
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
        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("message")]
        public string? Message { get; set; }

        [JsonProperty("data")]
        public PaystackData? Data { get; set; }
    }

    public class PaystackData
    {
        [JsonProperty("authorization_url")]
        public string? AuthorizationUrl { get; set; }

        [JsonProperty("access_code")]
        public string? AccessCode { get; set; }

        [JsonProperty("reference")]
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
        [JsonProperty("status")]
        public string? Status { get; set; }

        [JsonProperty("message")]
        public string? Message { get; set; }

        [JsonProperty("data")]
        public Data? Data { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("link")]
        public string? Link { get; set; }
    }
}
