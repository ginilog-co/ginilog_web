namespace Genilog_WebApi.Key
{
    public class Cls_Keys : IDisposable
    {

        private static string apiKey = "AIzaSyBPmZHEnuc80XMDyPe3KeTYioWH_ZVj3sk";
        private static string bucket = "ginilog-e3c8a.firebaseapp.com";
        private static string bucketFile = "ginilog-e3c8a.appspot.com";
        private static string projectId = "ginilog-e3c8a";
        private static string accessToken = "701313096073-vt5nojr1jtkruunavs2mhvk3d7ssfrk2.apps.googleusercontent.com";
        private static string accessId = "G-5W6Z8JTRN1";
        private static string messeageSendId = "701313096073";
        private static string cloudeMessageKey = "701313096073";
        private static string appId = "1:701313096073:web:34be72968f9a14d183e958";
        private static string paystackSecretKey1 = "sk_live_453f6643d0ae38a02c94d1e20bc9de2231f9b8ff";
        private static string paystackSecretKey = "sk_test_bddced709bd1dc7069ed81c77644f531cc86cb74";  
        private static string flutterwaveSecretKey1 = "sk_live_453f6643d0ae38a02c94d1e20bc9de2231f9b8ff";
        private static string flutterwaveSecretKey = "FLWSECK_TEST-b56f6e352ec72e745191f44c5d0575dd-X";
        private static string sitURl = "https://localhost:7133";
        private static string sitURl1 = "https://api-data.ginilog.com";


        public static string ApiKey { get => apiKey; set => apiKey = value; }
        public static string Bucket { get => bucket; set => bucket = value; }
        public static string BucketFile { get => bucketFile; set => bucketFile = value; }
        public static string ProjectId { get => projectId; set => projectId = value; }
        public static string AccessToken { get => accessToken; set => accessToken = value; }
        public static string AccessId { get => accessId; set => accessId = value; }
        public static string MesseageSendId { get => messeageSendId; set => messeageSendId = value; }
        public static string CloudeMessageKey { get => cloudeMessageKey; set => cloudeMessageKey = value; }
        public static string AppId { get => appId; set => appId = value; }
        public static string PaystackSecretKey { get => paystackSecretKey; set => paystackSecretKey = value; }
        public static string FlutterwaveSecretKey { get => flutterwaveSecretKey; set => flutterwaveSecretKey = value; }
        public static string ServerURL { get => sitURl; set => sitURl = value; }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
