using Microsoft.Extensions.Options;

namespace Genilog_WebApi.Key
{
    public class PaymentConfig
    {
        public string? Environment { get; set; } = "Test"; // Default to Test
        public string? PaystackTestSK { get; set; }
        public string? PaystackLiveSK { get; set; }
        public string? FlutterwaveTestSK { get; set; }
        public string? FlutterwaveLiveSK { get; set; }
        public string? Monnify { get; set; }

        // Helper properties to automatically pick the correct key
        public string? PaystackSK => Environment!.Equals("Live", StringComparison.OrdinalIgnoreCase)
                                    ? PaystackLiveSK
                                    : PaystackTestSK;

        public string? FlutterwaveSK => Environment!.Equals("Live", StringComparison.OrdinalIgnoreCase)
                                      ? FlutterwaveLiveSK
                                      : FlutterwaveTestSK;
    }

    public class FirebaseConfig
    {
        public string? ApiKey { get; set; }
        public string? AuthDomain { get; set; }
        public string? ProjectId { get; set; }
        public string? StorageBucket { get; set; }
        public string? MessagingSenderId { get; set; }
        public string? AppId { get; set; }
        public string? MeasurementId { get; set; }
    }

    public class ServerConfig
    {
        public string? Environment { get; set; } = "Test"; // Default to Test
        public string? TestBaseUrl { get; set; }
        public string? LiveBaseUrl { get; set; }


        public string? BaseUrl => Environment!.Equals("Live", StringComparison.OrdinalIgnoreCase)
                              ? LiveBaseUrl
                              : TestBaseUrl;
    }
    public class Cls_Keys(
        IOptions<PaymentConfig> paymentOptions,
        IOptions<FirebaseConfig> firebaseOptions,
        IOptions<ServerConfig> serverOptions) : IDisposable
    {
        // These are now populated from appsettings.json / env variables
        private PaymentConfig Payment { get; } = paymentOptions.Value;
        private FirebaseConfig Firebase { get; } = firebaseOptions.Value;
        private ServerConfig Server { get; } = serverOptions.Value;

        // Example of helper properties to replace old static fields
        public string ApiKey => Firebase.ApiKey!;
        public string Bucket => Firebase.StorageBucket!;
        public string BucketFile => Firebase.StorageBucket!;
        public string ProjectId => Firebase.ProjectId!;
        public string AccessToken => Firebase.MessagingSenderId!;
        public string AccessId => Firebase.AppId!;
        public string MesseageSendId => Firebase.MessagingSenderId!;
        public string CloudeMessageKey => Firebase.MessagingSenderId!;
        public string AppId => Firebase.AppId!;

        // Auto picks Test or Live keys based on Payment.Environment
        public string PaystackSecretKey => Payment.PaystackSK!;
        public string FlutterwaveSecretKey => Payment.FlutterwaveSK!;

        public string ServerURL => Server.BaseUrl!;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
