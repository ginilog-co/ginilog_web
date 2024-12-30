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

 
        public static string ApiKey { get => apiKey; set => apiKey = value; }
        public static string Bucket { get => bucket; set => bucket = value; }
        public static string BucketFile { get => bucketFile; set => bucketFile = value; }
        public static string ProjectId { get => projectId; set => projectId = value; }
        public static string AccessToken { get => accessToken; set => accessToken = value; }
        public static string AccessId { get => accessId; set => accessId = value; }
        public static string MesseageSendId { get => messeageSendId; set => messeageSendId = value; }
        public static string CloudeMessageKey { get => cloudeMessageKey; set => cloudeMessageKey = value; }
        public static string AppId { get => appId; set => appId = value; }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
