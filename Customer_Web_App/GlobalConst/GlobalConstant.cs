namespace Customer_Web_App.GlobalConst
{
    public class GlobalConstant
    {
        static string baseUrl = "https://api-data.ginilog.com/api/";

        static string usersUrl = "AuthUsers";
        static string usersLoginUrl = "AuthUsers/login";
        static string emailVerificationUrl = "AuthUsers/email-verification";
        static string phoneVerificationUrl = "AuthUsers/phone-no-verification";
        static string phoneNoVerificationUrl = "AuthUsers/PhoneNo-Verification";
        static string twoFactorEnabledUrl = "AuthUsers/two-factor-enabled";
        static string forgetPasswordUrl = "AuthUsers/forgot-password-request-token";
        static string resetPasswordUrl = "AuthUsers/reset-password";
        static string userUpdate = "AuthUsers/update-user";
        static string uploadUrl = "UploadFile/upload-web-server";
        static string resendEmailVerificationTokenUrl = "AuthUsers/email-verification-request-token";
        static string contactUsUrl = "Info/ContactUs";
        //Flutterwave
        static string flutterWaveKey = "FLWPUBK_TEST-0401652f50334af315d414a6568bdf5f-X";

      //  static string flutterWaveKey = "FLWPUBK_TEST-2624c0cbf9db0abffb95401130be6432-X";
        // Paystack
        static string paystackSecretKey = "sk_test_bddced709bd1dc7069ed81c77644f531cc86cb74";

      //  static string paystackSecretKey = "sk_test_bddced709bd1dc7069ed81c77644f531cc86cb74";

        static string paystackPublicKey = "pk_test_22bfc066e91d081926d5d5fa7701770dd90ce89a";

     //   static string paystackPublicKey = "pk_test_22bfc066e91d081926d5d5fa7701770dd90ce89a";

        // Admin Management
        public static string BaseUrl { get => baseUrl; set => baseUrl = value; }
        public static string UserUrl { get => usersUrl; set => usersUrl = value; }
        public static string UserLoginUrl { get => usersLoginUrl; set => usersLoginUrl = value; }
        public static string EmailVerificationUrl { get => emailVerificationUrl; set => emailVerificationUrl = value; }
        public static string PhoneVerificationUrl { get => phoneVerificationUrl; set => phoneVerificationUrl = value; }
        public static string PhoneNoVerificationUrl { get => phoneNoVerificationUrl; set => phoneNoVerificationUrl = value; }
        public static string TwoFactorEnabledUrl { get => twoFactorEnabledUrl; set => twoFactorEnabledUrl = value; }
        public static string ForgetPasswordUrl { get => forgetPasswordUrl; set => forgetPasswordUrl = value; }
        public static string ResetPasswordUrl { get => resetPasswordUrl; set => resetPasswordUrl = value; }
        public static string UserUpdate { get => userUpdate; set => userUpdate = value; }
        public static string UploadUrl { get => uploadUrl; set => uploadUrl = value; }
        public static string ResendEmailVerificationTokenUrl { get => resendEmailVerificationTokenUrl; set => resendEmailVerificationTokenUrl = value; }
        public static string FlutterWaveKey { get => flutterWaveKey; set => flutterWaveKey = value; }
        public static string PaystackSecretKey { get => paystackSecretKey; set => paystackSecretKey = value; }
        public static string PaystackPublicKey { get => paystackPublicKey; set => paystackPublicKey = value; }
        // Info Management
        public static string ContactUsUrl { get => contactUsUrl; set => contactUsUrl = value; }
    }
}
