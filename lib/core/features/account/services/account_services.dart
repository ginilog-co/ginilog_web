import 'dart:convert';
import 'package:ginilog_customer_app/core/components/extension/error_handling.dart';
import 'package:http/http.dart' as http;
import '../../../components/helpers/endpoints.dart';
import '../../../components/helpers/globals.dart';
import '../../../components/utils/constants.dart';

import '../model/user_response_model.dart';

class AccountService {
  Future<RegisterResponseModel> getUserData() async {
    RegisterResponseModel data = RegisterResponseModel();
    try {
      Map<String, String> headers2 = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
      };
      var url = Uri.parse("${Endpoints.baseUrl}${Endpoints.usersUrl}/profile");
      var response = await http.get(url, headers: headers2);
      if (response.statusCode == 200 || response.statusCode == 201) {
        final item = json.decode(response.body);
        data = RegisterResponseModel.fromJson(
          item,
        ); // Mapping json response to our data model
        printData('User Details', data);
        return data;
      } else {
        printData('User Details Error', response.body);
        return data;
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }

  static Future<RegisterResponseModel> browse() async {
    RegisterResponseModel data = RegisterResponseModel();
    try {
      Map<String, String> headers2 = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
      };
      var url = Uri.parse(
        "${Endpoints.baseUrl}${Endpoints.usersUrl}/${globals.userId}",
      );
      var response = await http.get(url, headers: headers2);
      if (response.statusCode == 200 || response.statusCode == 201) {
        final item = json.decode(response.body);
        data = RegisterResponseModel.fromJson(
          item,
        ); // Mapping json response to our data model
        printData('User Details', data);
        return data;
      } else {
        printData('User Details Error', response.body);
        return data;
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }

  Future<http.Response> updateProfile({
    String? firstName,
    String? lastName,
    String? imageFile,
    String? phoneNo,
    bool? availability,
  }) async {
    try {
      var stingUrl = Uri.parse("${Endpoints.baseUrl}${Endpoints.userUpdate}");
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
      };
      final msg = jsonEncode({
        "firstName": firstName ?? "",
        "lastName": lastName ?? "",
        "profilePicture": imageFile ?? "",
        "phoneNo": phoneNo ?? "",
        "userStatus": availability ?? false,
      });
      http.Response response = await http.put(
        stingUrl,
        body: msg,
        headers: headers,
      );

      if (response.statusCode == 200 || response.statusCode == 201) {
        printData("Response", response.body);
        getUserData();
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

  Future<http.Response> addNewAddress({
    required String userId,
    required String address,
    required String addressPostCodes,
    required String houseNo,
    required String city,
    required String state,
    required double latitude,
    required double longitude,
    required String phoneNo,
    required String userName,
  }) async {
    try {
      var stingUrl = Uri.parse(
        "${Endpoints.baseUrl}${Endpoints.usersUrl}/add-new-address/$userId",
      );
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
      };
      final msg = jsonEncode({
        "address": address,
        "phoneNo": phoneNo,
        "userName": userName,
        "addressPostCodes": addressPostCodes,
        "houseNo": houseNo,
        "city": city,
        "state": state,
        "latitude": latitude,
        "longitude": longitude,
      });
      http.Response response = await http.put(
        stingUrl,
        body: msg,
        headers: headers,
      );

      if (response.statusCode == 200 || response.statusCode == 201) {
        setToLocalStorage(name: "state", data: state);
        setToLocalStorage(name: "city", data: city);
        setToLocalStorage(name: "address", data: address);
        setDoubleToLocalStorage(name: "latitude", data: latitude);
        setDoubleToLocalStorage(name: "longitude", data: longitude);
        await globals.init();
        printData("Response", response.body);
        getUserData();
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

  Future<http.Response> updateAddress({
    required String addressId,
    required String address,
    required String addressPostCodes,
    required String houseNo,
    required String city,
    required double latitude,
    required double longitude,
    required String phoneNo,
    required String userName,
  }) async {
    try {
      var stingUrl = Uri.parse(
        "${Endpoints.baseUrl}${Endpoints.usersUrl}/update-delivery-address/$addressId",
      );
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
      };
      final msg = jsonEncode({
        "phoneNo": phoneNo,
        "userName": userName,
        "address": address,
        "addressPostCodes": addressPostCodes,
        "houseNo": houseNo,
        "city": city,
        "latitude": latitude,
        "longitude": longitude,
      });
      http.Response response = await http.put(
        stingUrl,
        body: msg,
        headers: headers,
      );

      if (response.statusCode == 200 || response.statusCode == 201) {
        setToLocalStorage(name: "city", data: city);
        setToLocalStorage(name: "address", data: address);
        setDoubleToLocalStorage(name: "latitude", data: latitude);
        setDoubleToLocalStorage(name: "longitude", data: longitude);
        await globals.init();
        printData("Response", response.body);
        getUserData();
        return response;
      } else {
        printData("Error", response.body);
        printData("Error", response.statusCode);
        return response;
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }

  Future<http.Response> deleteDeliveryAddress({
    required String addressId,
  }) async {
    try {
      var stingUrl = Uri.parse(
        "${Endpoints.baseUrl}${Endpoints.usersUrl}/delete-delivery-address/$addressId",
      );
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
      };
      http.Response response = await http.delete(stingUrl, headers: headers);
      if (response.statusCode == 200 || response.statusCode == 201) {
        printData("Response", response.body);
        getUserData();
        return response;
      } else {
        printData("Error", response.body);
        return response;
      }
    } catch (e) {
      rethrow;
    }
  }

  Future<http.Response> sendFeedBack({
    required String feedback,
    required String phoneNo,
  }) async {
    try {
      var stingUrl = Uri.parse("${Endpoints.baseUrl}Info/feedback");
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
      };
      final msg = jsonEncode({
        "name": globals.userName,
        "email": globals.userEmail,
        "feedback": feedback,
        "phoneNo": phoneNo,
      });
      http.Response response = await http.post(
        stingUrl,
        body: msg,
        headers: headers,
      );

      if (response.statusCode == 200 || response.statusCode == 201) {
        printData("Response", response.body);
        getUserData();
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
