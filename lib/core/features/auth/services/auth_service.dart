// ignore_for_file: use_build_context_synchronously

import 'dart:convert';

import 'package:ginilog_customer_app/core/components/extension/error_handling.dart';
import 'package:http/http.dart' as http;
import 'package:sign_in_with_apple/sign_in_with_apple.dart';

import '../../../components/helpers/endpoints.dart';
import '../../../components/helpers/globals.dart';
import '../../../components/utils/constants.dart';
import '../../../components/utils/helper_functions.dart';
import '../../../components/utils/package_export.dart';
import '../controller/confirm_email.dart';
import '../model/login_response_model.dart';
import '../model/register_model.dart';

class AuthService {
  // Google SignIn
  final GoogleSignIn _googleSignIn = GoogleSignIn(
    scopes: ['email', 'profile'],
  );

  Future<bool> signInWithGoogle() async {
    try {
      final GoogleSignInAccount? googleUser = await _googleSignIn.signIn();
      if (googleUser != null) {
        final GoogleSignInAuthentication googleAuth =
            await googleUser.authentication;
        final String? idToken = googleAuth.idToken;
        Map<String, String> headers = {
          'Content-Type': 'application/json',
          'Accept': 'application/json',
        };
        var url = Uri.parse("${Endpoints.baseUrl}google");
        final msg = jsonEncode(
          {
            "idToken": idToken,
          },
        );
        http.Response response =
            await http.post(url, body: msg, headers: headers);
        printData("Status code", response.statusCode);

        if (response.statusCode == 200 || response.statusCode == 201) {
          LoginResponseModel loginResponseModel =
              LoginResponseModel.fromJson(jsonDecode(response.body));
          setToLocalStorage(name: "token", data: loginResponseModel.token);
          //  setToLocalStorage(name: "userPassword", data: password);
          setToLocalStorage(name: "userEmail", data: loginResponseModel.email);
          setToLocalStorage(name: "userId", data: loginResponseModel.userId);
          setToLocalStorage(
              name: "userName", data: loginResponseModel.fullName);
          setToLocalStorage(
              name: "profilePicture", data: loginResponseModel.profileImage);
          await globals.init();
          return true;
        } else {
          printData("Authenticate", 'Failed to authenticate');
          return false;
        }
      } else {
        printData("Authenticate", 'Failed to authenticate$googleUser');
        return false;
      }
    } catch (error) {
      printData("Authenticate", 'Error signing in with Google: $error');
      return false;
    }
  }

//Apple SignIn
  Future<bool> signInWithApple() async {
    try {
      final credential = await SignInWithApple.getAppleIDCredential(
        scopes: [
          AppleIDAuthorizationScopes.email,
          AppleIDAuthorizationScopes.fullName,
        ],
      );
      final String? idToken = credential.identityToken;
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      };
      var url = Uri.parse("${Endpoints.baseUrl}apple");
      final msg = jsonEncode(
        {
          "idToken": idToken,
        },
      );
      http.Response response =
          await http.post(url, body: msg, headers: headers);
      printData("Status code", response.statusCode);

      if (response.statusCode == 200 || response.statusCode == 201) {
        LoginResponseModel loginResponseModel =
            LoginResponseModel.fromJson(jsonDecode(response.body));
        setToLocalStorage(name: "token", data: loginResponseModel.token);
        //  setToLocalStorage(name: "userPassword", data: password);
        setToLocalStorage(name: "userEmail", data: loginResponseModel.email);
        setToLocalStorage(name: "userId", data: loginResponseModel.userId);
        setToLocalStorage(name: "userName", data: loginResponseModel.fullName);
        setToLocalStorage(
            name: "profilePicture", data: loginResponseModel.profileImage);
        await globals.init();
        return true;
      } else {
        printData("Authenticate", 'Failed to authenticate');
        return false;
      }
    } catch (error) {
      printData("Authenticate", 'Error signing in with Google: $error');
      return false;
    }
  }

// Normal SignIn
  static Future<bool> register(
      {RegisterModel? registerModel, required BuildContext? cxt}) async {
    try {
      var stingUrl = Uri.parse(Endpoints.baseUrl + Endpoints.usersUrl);
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      };
      final msg = jsonEncode(
        {
          "lastName": registerModel!.lastName,
          "firstName": registerModel.firstName,
          "email": registerModel.email,
          "password": registerModel.password,
          "phoneNo": registerModel.phoneNo,
        },
      );
      http.Response response =
          await http.post(stingUrl, body: msg, headers: headers);
      printData("dataResponse", response.body);
      if (response.statusCode == 200 || response.statusCode == 201) {
        printData("successRegister", response.body);

        return true;
      } else {
        printData("Error", response.body);
        return false;
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }

  static Future<bool> login(
      {required String email,
      required String password,
      required BuildContext? cxt}) async {
    try {
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      };
      var url = Uri.parse("${Endpoints.baseUrl}${Endpoints.usersLoginUrl}");
      final msg = jsonEncode(
        {
          "email_PhoneNo": email,
          "password": password,
        },
      );
      http.Response response =
          await http.post(url, body: msg, headers: headers);
      printData("Status code", response.statusCode);
      if (response.statusCode == 200 || response.statusCode == 201) {
        LoginResponseModel loginResponseModel =
            LoginResponseModel.fromJson(jsonDecode(response.body));
        setToLocalStorage(name: "token", data: loginResponseModel.token);
        setToLocalStorage(name: "userPassword", data: password);
        setToLocalStorage(name: "userEmail", data: loginResponseModel.email);
        setToLocalStorage(name: "userId", data: loginResponseModel.userId);
        setToLocalStorage(name: "userName", data: loginResponseModel.fullName);
        setToLocalStorage(
            name: "profilePicture", data: loginResponseModel.profileImage);
        setBoolToLocalStorage(name: "isHomeLoaded", data: true);
        setBoolToLocalStorage(
            name: "isEmailVerified", data: loginResponseModel.emailVerified);
        printData("Login Success", loginResponseModel);

        globals.init();
        return true;
      } else if (response.body == "User Email Not Yet Verify") {
        ScaffoldMessenger.of(cxt!).showSnackBar(
            const SnackBar(content: Text("New code sent to your email")));
        navigateAndRemoveUntilRoute(
            cxt,
            ConfirmEmailAddressScreen(
              email: email,
              fromLogin: false,
              password: password,
            ));
        return false;
      } else {
        printData("Error", response.body);

        return false;
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }

  Future<bool> userLogin(
      {required String email,
      required String password,
      required BuildContext? cxt}) async {
    try {
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      };
      var url = Uri.parse("${Endpoints.baseUrl}${Endpoints.usersLoginUrl}");
      final msg = jsonEncode(
        {
          "email_PhoneNo": email,
          "password": password,
        },
      );
      http.Response response =
          await http.post(url, body: msg, headers: headers);
      printData("email", email);
      printData("password", password);
      printData("Status code", url);
      printData("Status code", response.statusCode);

      if (response.statusCode == 200 || response.statusCode == 201) {
        LoginResponseModel loginResponseModel =
            LoginResponseModel.fromJson(jsonDecode(response.body));
        setToLocalStorage(name: "token", data: loginResponseModel.token);
        setToLocalStorage(name: "userPassword", data: password);
        setToLocalStorage(name: "userEmail", data: loginResponseModel.email);
        setToLocalStorage(name: "userId", data: loginResponseModel.userId);
        setToLocalStorage(name: "userName", data: loginResponseModel.fullName);
        setToLocalStorage(
            name: "profilePicture", data: loginResponseModel.profileImage);
        setBoolToLocalStorage(name: "isHomeLoaded", data: true);
        setBoolToLocalStorage(
            name: "isEmailVerified", data: loginResponseModel.emailVerified);
        await globals.init();
        return true;
      } else {
        printData("errors", response.body);
        return false;
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }

  static verifyEmail(
      {required String password, pin, required BuildContext? cxt}) async {
    try {
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      };
      var url =
          Uri.parse("${Endpoints.baseUrl}${Endpoints.emailVerificationUrl}");
      final msg = jsonEncode(
        {
          "token": pin,
          "password": password,
        },
      );
      http.Response response =
          await http.post(url, body: msg, headers: headers);
      printData("dataResponse", response.body);
      if (response.statusCode == 200 || response.statusCode == 201) {
        LoginResponseModel loginResponseModel =
            LoginResponseModel.fromJson(jsonDecode(response.body));
        setToLocalStorage(name: "userPassword", data: password);
        setToLocalStorage(name: "token", data: loginResponseModel.token);
        setToLocalStorage(name: "userEmail", data: loginResponseModel.email);
        setToLocalStorage(name: "userId", data: loginResponseModel.userId);
        setToLocalStorage(name: "userName", data: loginResponseModel.fullName);
        setToLocalStorage(
            name: "profilePicture", data: loginResponseModel.profileImage);
        setBoolToLocalStorage(
            name: "isEmailVerified", data: loginResponseModel.emailVerified);
        setToLocalStorage(name: "isLoggedIn", data: "isLoggedIn");
        await globals.init();
        printData("verify", response.body);
        //navigateToRoute(cxt, Ta)
      } else {
        printData("errors", response.body);
      }
      return response;
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }

  static sendVerificationCode(
      {required String email, required BuildContext? cxt}) async {
    var url = Uri.parse(
        "${Endpoints.baseUrl}${Endpoints.resendEmailVerificationTokenUrl}");
    Map<String, String> headers = {
      'Content-Type': 'application/json',
      'Accept': 'application/json',
    };
    try {
      final msg = jsonEncode(
        {
          "email": email,
        },
      );
      http.Response response =
          await http.post(url, body: msg, headers: headers);
      printData("dataResponse", response.body);
      if (response.statusCode == 200 || response.statusCode == 201) {
        printData("Response", response.body);
      } else {
        printData("Errors", response.body);
      }

      return response;
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }

  static reSendPasswordCode(
      {required String email, required BuildContext? cxt}) async {
    var url = Uri.parse("${Endpoints.baseUrl}${Endpoints.forgetPasswordUrl}");
    Map<String, String> headers = {
      'Content-Type': 'application/json',
      'Accept': 'application/json',
    };
    try {
      final msg = jsonEncode(
        {
          "email": email,
        },
      );
      http.Response response =
          await http.post(url, body: msg, headers: headers);
      printData("dataResponse", response.body);
      if (response.statusCode == 200 || response.statusCode == 201) {
        printData("Response", response.body);
      } else {
        printData("Errors", response.body);
      }

      return response;
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }

  static forgotPassword(
      {required String token,
      required String password,
      required BuildContext? cxt}) async {
    var url = Uri.parse("${Endpoints.baseUrl}${Endpoints.resetPasswordUrl}");
    Map<String, String> headers = {
      'Content-Type': 'application/json',
      'Accept': 'application/json',
    };
    try {
      final msg = jsonEncode(
        {"token": token, "password": password},
      );
      http.Response response =
          await http.post(url, body: msg, headers: headers);
      printData("dataResponse", response.body);
      printData("dataResponse", response.body);
      if (response.statusCode == 200 || response.statusCode == 201) {
        printData("Response", response.body);
      } else {
        printData("Errors", response.body);
      }
      return response;
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }

//KYC User Verifications
  Future<bool> updateProfileInfo({
    required String userId,
    required String sex,
    required String address,
    required String dateOfBirth,
    required String phoneNo,
    required String nationality,
    required String state,
    required String nextOfKin,
  }) async {
    try {
      var stingUrl =
          Uri.parse("${Endpoints.baseUrl}${Endpoints.usersUrl}/$userId");
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}'
      };
      final msg = jsonEncode(
        {
          "phoneNo": phoneNo,
          "sex": sex,
          "address": address,
          "dateOfBirth": dateOfBirth,
          "nationality": nationality,
          "state": state,
          "nextOfKin": nextOfKin,
        },
      );
      http.Response response =
          await http.put(stingUrl, body: msg, headers: headers);
      if (response.statusCode == 200 || response.statusCode == 201) {
        printData("Response", response.body);
        setToLocalStorage(name: "InfoCompleted", data: "Completed");
        await globals.init();
        return true;
      } else {
        printData("Error", response.body);
        printData("Error3", globals.userId);
        return false;
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }

  Future<bool> updateDeviceToken() async {
    try {
      var stingUrl = Uri.parse(
          "${Endpoints.baseUrl}${Endpoints.usersUrl}/update-device-token/${globals.userId}");
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}'
      };

      final msg = jsonEncode(
        {
          "deviceToken": globals.deviceToken,
        },
      );
      http.Response response =
          await http.put(stingUrl, body: msg, headers: headers);

      if (response.statusCode == 200 || response.statusCode == 201) {
        return true;
      } else {
        printData("Error", response.body);
        return false;
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }

  Future<bool> deleteUser() async {
    try {
      var stingUrl = Uri.parse(
          "${Endpoints.baseUrl}${Endpoints.usersUrl}/${globals.userId}");
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}'
      };
      http.Response response = await http.delete(stingUrl, headers: headers);

      if (response.statusCode == 200 || response.statusCode == 201) {
        printData("Deleted Account", response.body);
        return true;
      } else {
        printData("Delete Error", response.body);
        return false;
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }

  Future<bool> updateUserStatus(bool availability) async {
    try {
      var stingUrl = Uri.parse(
          "${Endpoints.baseUrl}${Endpoints.usersUrl}/update-user-status/${globals.userId}");
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}'
      };

      final msg = jsonEncode(
        {
          "availability": availability,
        },
      );
      http.Response response =
          await http.put(stingUrl, body: msg, headers: headers);

      if (response.statusCode == 200 || response.statusCode == 201) {
        return true;
      } else {
        printData("Error", response.body);
        return false;
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }
}
