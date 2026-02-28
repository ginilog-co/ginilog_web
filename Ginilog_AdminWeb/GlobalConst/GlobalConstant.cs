namespace Ginilog_AdminWeb.GlobalConst
{
    public class GlobalConstant
    {
        static string baseUrl = "https://api-data.ginilog.com/api/";

        // Admin Management
        private static string adminUrl = "Admin/";
        private static string adminLoginUrl = "Admin/login";
        private static string forgetPasswordUrl = "Admin/forgot-password-request-token";
        private static string ressetPasswordUrl = "Admin/reset-password";
        private static string profileImageUrl = "Admin/update-profile-image";

        // Info Management
        private static string contactUsUrl = "Info/feedback";

        // Admin Management
        public static string BaseUrl { get => baseUrl; set => baseUrl = value; }
        public static string AdminUrl { get => adminUrl; set => adminUrl = value; }
        public static string AdminLoginUrl { get => adminLoginUrl; set => adminLoginUrl = value; }
        public static string ForgetPasswordUrl { get => forgetPasswordUrl; set => forgetPasswordUrl = value; }
        public static string RessetPasswordUrl { get => ressetPasswordUrl; set => ressetPasswordUrl = value; }
        public static string ProfileImageUrl { get => profileImageUrl; set => profileImageUrl = value; }
        // Info Management
        public static string ContactUsUrl { get => contactUsUrl; set => contactUsUrl = value; }
    }
}
