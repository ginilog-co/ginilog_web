class Endpoints {
  static String baseUrl = "https://api-data-code.cntechlite.com/api/";

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
  static String flutterWaveTestKey =
      "FLWPUBK_TEST-2624c0cbf9db0abffb95401130be6432-X";
  static String flutterWaveLiveKey =
      "FLWPUBK_TEST-2624c0cbf9db0abffb95401130be6432-X";
}
