import 'dart:convert';

import 'package:ginilog_customer_app/core/components/constants/api_constants.dart';
import 'package:ginilog_customer_app/core/features/auth/services/auth_service.dart';

import '../../features/auth/model/login_response_model.dart';
import '../utils/constants.dart';
import '../utils/package_export.dart';
import 'endpoints.dart';
import 'package:http/http.dart' as http;

final GetIt getIt = GetIt.instance;

class AppGlobals {
  factory AppGlobals() => instance;

  AppGlobals._();

  static final AppGlobals instance = AppGlobals._();

  String? isLoggedIn;
  int? isViewed;
  StopWatchTimer? stopWatchTimer;
  String? userEmail = "";
  String? userPassword = "";
  String? token = "";
  String? userId = "";
  String? userName = "";
  String? deviceToken = "";
  String? profilePicture = "";
  bool? isHomeLoading = true;
  bool? isEmailVerified = false;

  Future<void> init() async {
    SharedPreferences preferences = await SharedPreferences.getInstance();
    userId = await getFromLocalStorage(name: "userId") ?? "";
    token = await getFromLocalStorage(name: "token") ?? "";
    userEmail = await getFromLocalStorage(name: "userEmail") ?? "";
    userPassword = await getFromLocalStorage(name: "userPassword") ?? "";
    userName = await getFromLocalStorage(name: "userName") ?? "";
    deviceToken = await getFromLocalStorage(name: "deviceToken") ?? "";
    isViewed = preferences.getInt('onBoard');
    isLoggedIn = preferences.getString('isLoggedIn') ?? "";

    isViewed = preferences.getInt('onBoard');
    isLoggedIn = preferences.getString('isLoggedIn') ?? "";
    isHomeLoading = await getBoolFromLocalStorage(name: "isHomeLoaded") ?? true;
    isEmailVerified =
        await getBoolFromLocalStorage(name: "isEmailVerified") ?? false;
    printData("userId", userId);
    printData("UserEmail", userEmail);
    printData("Token", token);
    printData("deviceToken", deviceToken);
  }

  Future<void> login() async {
    SharedPreferences preferences = await SharedPreferences.getInstance();
    String url = Endpoints.usersLoginUrl;
    var stingUrl = Uri.parse(Endpoints.baseUrl + url);
    String userPassword = await getFromLocalStorage(name: "userPassword") ?? "";
    try {
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      };
      final msg = jsonEncode({
        "email_PhoneNo": userEmail,
        "password": userPassword,
      });
      http.Response response =
          await http.post(stingUrl, body: msg, headers: headers);
      printData("dataResponse", response.body);
      printData("URL", stingUrl);
      if (response.statusCode == 200 || response.statusCode == 201) {
        LoginResponseModel loginResponseModel =
            LoginResponseModel.fromJson(jsonDecode(response.body));
        setToLocalStorage(name: "token", data: loginResponseModel.token);
        setToLocalStorage(name: "userEmail", data: loginResponseModel.email);
        setToLocalStorage(name: "userId", data: loginResponseModel.userId);

        setToLocalStorage(name: "userName", data: loginResponseModel.fullName);
        setToLocalStorage(
            name: "profilePicture", data: loginResponseModel.profileImage);
        setBoolToLocalStorage(name: "isHomeLoaded", data: true);
        setBoolToLocalStorage(
            name: "isEmailVerified", data: loginResponseModel.emailVerified);
        printData("verify", loginResponseModel.email);
        await init();
      } else if (response.statusCode == 401) {
        printData("Error", response.body);
        preferences.remove("token");
        preferences.remove("userEmail");
        preferences.remove("userId");
        preferences.remove("userPassword");
        preferences.remove("deviceToken");
        preferences.remove("state");
        preferences.remove("city");
        preferences.remove("address");
        preferences.remove("latitude");
        preferences.remove("longitude");
        preferences.remove("isHomeLoaded");
        preferences.remove("isEmailVerified");
        await init();
        await AuthService().updateDeviceToken();
      } else {
        printData("errors", response.body);
        printData("Error", response.body);
        preferences.remove("token");
        preferences.remove("userEmail");
        preferences.remove("userId");
        preferences.remove("userPassword");
        preferences.remove("deviceToken");
        preferences.remove("state");
        preferences.remove("city");
        preferences.remove("address");
        preferences.remove("latitude");
        preferences.remove("longitude");
        preferences.remove("isHomeLoaded");
        preferences.remove("isEmailVerified");
        await init();
      }
    } catch (e) {
      rethrow;
    }
  }

  updateHomeLoaded() async {
    setBoolToLocalStorage(name: "isHomeLoaded", data: false);

    await init();
    printData("IsHomeLoaded", isHomeLoading);
  }

  void dispose() {}
}

AppGlobals globals = getIt.get<AppGlobals>();
Future<void> setupLocator() async {
// Register dependencies

  getIt.registerLazySingleton(() => PushNotificationService());
}
