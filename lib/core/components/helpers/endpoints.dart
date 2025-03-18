class Endpoints {
  static String baseUrl = "https://api-data.ginilog.com/api/";

  static String usersUrl = "AuthUsers";
  static String usersLoginUrl = "AuthUsers/login";
  static String emailVerificationUrl = "AuthUsers/email-verification";
  static String phoneVerificationUrl = "AuthUsers/phone-no-verification";
  static String phoneNoVerificationUrl = "AuthUsers/PhoneNo-Verification";
  static String twoFactorEnabledUrl = "AuthUsers/two-factor-enabled";
  static String forgetPasswordUrl = "AuthUsers/forgot-password-request-token";
  static String resetPasswordUrl = "AuthUsers/reset-password";
  static String userUpdate = "AuthUsers/update-user";
  static String uploadUrl = "UploadFile/upload-web-server";
  static String resendEmailVerificationTokenUrl =
      "AuthUsers/email-verification-request-token";
  static String contactUsUrl = "Info/ContactUs";
  //Flutterwave
  static String flutterWaveKey =
      "FLWPUBK_TEST-0401652f50334af315d414a6568bdf5f-X";

  static String flutterWaveTestedKey =
      "FLWPUBK_TEST-0401652f50334af315d414a6568bdf5f-X";
  static String flutterWaveLiveEdKey =
      "FLWPUBK_TEST-2624c0cbf9db0abffb95401130be6432-X";
  // Paystack
  static String paystackSecretKey =
      "sk_test_bddced709bd1dc7069ed81c77644f531cc86cb74";
  static String paystackPublicKey =
      "pk_test_22bfc066e91d081926d5d5fa7701770dd90ce89a";
  static String paystackSecretTestedKey =
      "sk_test_bddced709bd1dc7069ed81c77644f531cc86cb74";
  static String paystackPublicTestedKey =
      "pk_test_22bfc066e91d081926d5d5fa7701770dd90ce89a";
  static String paystackSecretLiveKey =
      "FLWPUBK_TEST-2624c0cbf9db0abffb95401130be6432-X";
  static String paystackPublicLiveKey =
      "FLWPUBK_TEST-2624c0cbf9db0abffb95401130be6432-X";
}
