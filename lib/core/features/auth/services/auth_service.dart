// ignore_for_file: use_build_context_synchronously

import 'dart:convert';
import 'dart:math';

import 'package:ginilog_customer_app/core/components/extension/error_handling.dart';
import 'package:http/http.dart' as http;
import 'package:sign_in_with_apple/sign_in_with_apple.dart';

import '../../../components/helpers/endpoints.dart';
import '../../../components/helpers/globals.dart';
import '../../../components/utils/constants.dart';
import '../../../components/utils/package_export.dart';
import '../model/login_response_model.dart';
import '../model/register_model.dart';

class AuthService {
  final FirebaseAuth firebaseAuth = FirebaseAuth.instance;
  final User? result = FirebaseAuth.instance.currentUser;
  final GoogleSignIn googleSignIn = GoogleSignIn();
  final SignInWithApple appleSignIn = SignInWithApple();
  // Google Sign
  Future<http.Response> signInWithGoogle() async {
    try {
      final GoogleSignInAccount? googleUser = await googleSignIn.signIn();
      if (googleUser != null) {
        final GoogleSignInAuthentication googleAuth =
            await googleUser.authentication;
        final credential = GoogleAuthProvider.credential(
          accessToken: googleAuth.accessToken,
          idToken: googleAuth.idToken,
        );
        User? firebaseUser =
            (await firebaseAuth.signInWithCredential(credential)).user;
        String fullName = firebaseUser?.displayName ?? "";
        List<String> nameParts = fullName.split(" ");
        String firstName = nameParts.isNotEmpty ? nameParts.first : "";
        String lastName =
            nameParts.length > 1 ? nameParts.sublist(1).join(" ") : "";
        Map<String, String> headers = {
          'Content-Type': 'application/json',
          'Accept': 'application/json',
        };
        var url = Uri.parse("${Endpoints.baseUrl}AuthUsers/auth-login");
        final msg = jsonEncode({
          "email": firebaseUser?.email,
          "idToken": firebaseUser?.uid,
          "externalId": firebaseUser?.uid,
          "firstName": firstName,
          "lastName": lastName.isNotEmpty ? lastName : "LastName",
          "profilePicture": firebaseUser?.photoURL ?? "",
          "phoneNo": firebaseUser?.phoneNumber ?? "0",
        });
        http.Response response = await http.post(
          url,
          body: msg,
          headers: headers,
        );
        printData("Status code", response.statusCode);

        if (response.statusCode == 200 || response.statusCode == 201) {
          LoginResponseModel loginResponseModel = LoginResponseModel.fromJson(
            jsonDecode(response.body),
          );
          setToLocalStorage(name: "token", data: loginResponseModel.token);
          setToLocalStorage(name: "userPassword", data: firebaseUser?.uid);
          setToLocalStorage(name: "userEmail", data: loginResponseModel.email);
          setToLocalStorage(name: "userId", data: loginResponseModel.userId);
          setToLocalStorage(
            name: "userName",
            data: loginResponseModel.fullName,
          );
          setToLocalStorage(
            name: "profilePicture",
            data: loginResponseModel.profileImage,
          );
          setBoolToLocalStorage(name: "isHomeLoaded", data: true);
          setBoolToLocalStorage(
            name: "isEmailVerified",
            data: loginResponseModel.emailVerified,
          );
          await globals.init();
          return response;
        } else {
          printData("Authenticate", 'Failed to authenticate');
          return response;
        }
      } else {
        printData("Authenticate", 'Failed to authenticate');
        return Future.error(handleHttpError("Failed to authenticate"));
      }
    } catch (error) {
      printData("Authenticate", 'Error signing in with Google: $error');
      return Future.error(handleHttpError(error));
    }
  }

  //Apple SignIn
  String sha256ofString(String input) {
    final bytes = utf8.encode(input);
    final digest = sha256.convert(bytes);
    return digest.toString();
  }

  String generateNonce([int length = 32]) {
    const charset =
        '0123456789ABCDEFGHIJKLMNOPQRSTUVXYZabcdefghijklmnopqrstuvwxyz-._';
    final random = Random.secure();
    return List.generate(
      length,
      (_) => charset[random.nextInt(charset.length)],
    ).join();
  }

  Future<http.Response> signInWithApple() async {
    try {
      final rawNonce = generateNonce();
      final nonce = sha256ofString(rawNonce);
      final appleCredential = await SignInWithApple.getAppleIDCredential(
        scopes: [
          AppleIDAuthorizationScopes.email,
          AppleIDAuthorizationScopes.fullName,
        ],
        nonce: nonce,
      );
      printData("Auth Code", appleCredential.authorizationCode);
      final oauthCredential = OAuthProvider(
        "apple.com",
      ).credential(idToken: appleCredential.identityToken, rawNonce: rawNonce);
      User? firebaseUser =
          (await firebaseAuth.signInWithCredential(oauthCredential)).user;
      String fullName = firebaseUser?.displayName ?? "";
      List<String> nameParts = fullName.split(" ");
      String firstName = nameParts.isNotEmpty ? nameParts.first : "";
      String lastName =
          nameParts.length > 1 ? nameParts.sublist(1).join(" ") : "";
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      };
      var url = Uri.parse("${Endpoints.baseUrl}AuthUsers/auth-login");
      final msg = jsonEncode({
        "email": firebaseUser?.email,
        "idToken": firebaseUser?.uid,
        "externalId": firebaseUser?.uid,
        "firstName": firstName,
        "lastName": lastName.isNotEmpty ? lastName : "LastName",
        "profilePicture": firebaseUser?.photoURL ?? "",
        "phoneNo": firebaseUser?.phoneNumber ?? "0",
      });
      http.Response response = await http.post(
        url,
        body: msg,
        headers: headers,
      );
      printData("Status code", response.statusCode);

      if (response.statusCode == 200 || response.statusCode == 201) {
        LoginResponseModel loginResponseModel = LoginResponseModel.fromJson(
          jsonDecode(response.body),
        );
        setToLocalStorage(name: "token", data: loginResponseModel.token);
        setToLocalStorage(name: "userPassword", data: firebaseUser?.uid);
        setToLocalStorage(name: "userEmail", data: loginResponseModel.email);
        setToLocalStorage(name: "userId", data: loginResponseModel.userId);
        setToLocalStorage(name: "userName", data: loginResponseModel.fullName);
        setToLocalStorage(
          name: "profilePicture",
          data: loginResponseModel.profileImage,
        );
        setBoolToLocalStorage(name: "isHomeLoaded", data: true);
        setBoolToLocalStorage(
          name: "isEmailVerified",
          data: loginResponseModel.emailVerified,
        );
        await globals.init();
        return response;
      } else {
        printData("Authenticate", 'Failed to authenticate');
        return response;
      }
    } catch (error) {
      printData("Authenticate", 'Error signing in with Google: $error');
      return Future.error(handleHttpError(error));
    }
  }

  // Normal SignIn
  Future<http.Response> register({
    RegisterModel? registerModel,
    required BuildContext? cxt,
  }) async {
    try {
      var stingUrl = Uri.parse(Endpoints.baseUrl + Endpoints.usersUrl);
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      };
      final msg = jsonEncode({
        "lastName": registerModel!.lastName,
        "firstName": registerModel.firstName,
        "email": registerModel.email,
        "password": registerModel.password,
        "phoneNo": registerModel.phoneNo,
      });
      http.Response response = await http.post(
        stingUrl,
        body: msg,
        headers: headers,
      );
      printData("dataResponse", response.body);
      if (response.statusCode == 200 || response.statusCode == 201) {
        printData("successRegister", response.body);

        return response;
      } else {
        printData("Error", response.body);
        return response;
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }

  Future<http.Response> userLogin({
    required String email,
    required String password,
    required BuildContext? cxt,
  }) async {
    try {
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      };
      var url = Uri.parse("${Endpoints.baseUrl}${Endpoints.usersLoginUrl}");
      final msg = jsonEncode({"email_PhoneNo": email, "password": password});
      http.Response response = await http.post(
        url,
        body: msg,
        headers: headers,
      );
      printData("email", email);
      printData("password", password);
      printData("Status code", url);
      printData("Status code", response.statusCode);

      if (response.statusCode == 200 || response.statusCode == 201) {
        LoginResponseModel loginResponseModel = LoginResponseModel.fromJson(
          jsonDecode(response.body),
        );
        setToLocalStorage(name: "token", data: loginResponseModel.token);
        setToLocalStorage(name: "userPassword", data: password);
        setToLocalStorage(name: "userEmail", data: loginResponseModel.email);
        setToLocalStorage(name: "userId", data: loginResponseModel.userId);
        setToLocalStorage(name: "userName", data: loginResponseModel.fullName);
        setToLocalStorage(
          name: "profilePicture",
          data: loginResponseModel.profileImage,
        );
        setBoolToLocalStorage(name: "isHomeLoaded", data: true);
        setBoolToLocalStorage(
          name: "isEmailVerified",
          data: loginResponseModel.emailVerified,
        );
        await globals.init();
        return response;
      } else {
        printData("errors", response.body);
        return response;
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }

  Future<http.Response> verifyEmail({
    required String password,
    pin,
    required BuildContext? cxt,
  }) async {
    try {
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      };
      var url = Uri.parse(
        "${Endpoints.baseUrl}${Endpoints.emailVerificationUrl}",
      );
      final msg = jsonEncode({"token": pin, "password": password});
      http.Response response = await http.post(
        url,
        body: msg,
        headers: headers,
      );
      printData("dataResponse", response.body);
      if (response.statusCode == 200 || response.statusCode == 201) {
        LoginResponseModel loginResponseModel = LoginResponseModel.fromJson(
          jsonDecode(response.body),
        );
        setToLocalStorage(name: "userPassword", data: password);
        setToLocalStorage(name: "token", data: loginResponseModel.token);
        setToLocalStorage(name: "userEmail", data: loginResponseModel.email);
        setToLocalStorage(name: "userId", data: loginResponseModel.userId);
        setToLocalStorage(name: "userName", data: loginResponseModel.fullName);
        setToLocalStorage(
          name: "profilePicture",
          data: loginResponseModel.profileImage,
        );
        setBoolToLocalStorage(
          name: "isEmailVerified",
          data: loginResponseModel.emailVerified,
        );
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

  Future<http.Response> sendVerificationCode({
    required String email,
    required BuildContext? cxt,
  }) async {
    var url = Uri.parse(
      "${Endpoints.baseUrl}${Endpoints.resendEmailVerificationTokenUrl}",
    );
    Map<String, String> headers = {
      'Content-Type': 'application/json',
      'Accept': 'application/json',
    };
    try {
      final msg = jsonEncode({"email": email});
      http.Response response = await http.post(
        url,
        body: msg,
        headers: headers,
      );
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

  Future<http.Response> reSendPasswordCode({
    required String email,
    required BuildContext? cxt,
  }) async {
    var url = Uri.parse("${Endpoints.baseUrl}${Endpoints.forgetPasswordUrl}");
    Map<String, String> headers = {
      'Content-Type': 'application/json',
      'Accept': 'application/json',
    };
    try {
      final msg = jsonEncode({"email": email});
      http.Response response = await http.post(
        url,
        body: msg,
        headers: headers,
      );
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

  Future<http.Response> forgotPassword({
    required String token,
    required String password,
    required BuildContext? cxt,
  }) async {
    var url = Uri.parse("${Endpoints.baseUrl}${Endpoints.resetPasswordUrl}");
    Map<String, String> headers = {
      'Content-Type': 'application/json',
      'Accept': 'application/json',
    };
    try {
      final msg = jsonEncode({"token": token, "password": password});
      http.Response response = await http.post(
        url,
        body: msg,
        headers: headers,
      );
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
  Future<http.Response> updateProfileInfo({
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
      var stingUrl = Uri.parse(
        "${Endpoints.baseUrl}${Endpoints.usersUrl}/$userId",
      );
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
      };
      final msg = jsonEncode({
        "phoneNo": phoneNo,
        "sex": sex,
        "address": address,
        "dateOfBirth": dateOfBirth,
        "nationality": nationality,
        "state": state,
        "nextOfKin": nextOfKin,
      });
      http.Response response = await http.put(
        stingUrl,
        body: msg,
        headers: headers,
      );
      if (response.statusCode == 200 || response.statusCode == 201) {
        printData("Response", response.body);
        setToLocalStorage(name: "InfoCompleted", data: "Completed");
        await globals.init();
        return response;
      } else {
        printData("Error", response.body);
        printData("Error3", globals.userId);
        return response;
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }

  Future<http.Response> updateDeviceToken() async {
    try {
      var stingUrl = Uri.parse(
        "${Endpoints.baseUrl}${Endpoints.usersUrl}/update-device-token",
      );
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
      };

      final msg = jsonEncode({"deviceToken": globals.deviceToken});
      http.Response response = await http.put(
        stingUrl,
        body: msg,
        headers: headers,
      );

      if (response.statusCode == 200 || response.statusCode == 201) {
        return response;
      } else {
        printData("Device Token Error", response.body);
        return response;
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }

  Future<http.Response> deleteUser() async {
    try {
      var stingUrl = Uri.parse(
        "${Endpoints.baseUrl}${Endpoints.usersUrl}/${globals.userId}",
      );
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
      };
      http.Response response = await http.delete(stingUrl, headers: headers);

      if (response.statusCode == 200 || response.statusCode == 201) {
        printData("Deleted Account", response.body);
        return response;
      } else {
        printData("Delete Error", response.body);
        return response;
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }

  Future<http.Response> updateUserStatus(bool availability) async {
    try {
      var stingUrl = Uri.parse(
        "${Endpoints.baseUrl}${Endpoints.usersUrl}/update-user-status/${globals.userId}",
      );
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
      };

      final msg = jsonEncode({"availability": availability});
      http.Response response = await http.put(
        stingUrl,
        body: msg,
        headers: headers,
      );

      if (response.statusCode == 200 || response.statusCode == 201) {
        return response;
      } else {
        printData("Error", response.body);
        return response;
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }
}
