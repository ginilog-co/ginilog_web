import 'dart:convert';

import 'package:ginilog_customer_app/core/components/extension/error_handling.dart';
import 'package:ginilog_customer_app/core/components/helpers/endpoints.dart';
import 'package:ginilog_customer_app/core/components/helpers/globals.dart';
import 'package:ginilog_customer_app/core/components/utils/constants.dart';
import 'package:ginilog_customer_app/core/features/home/model/company_response_model.dart';
import 'package:ginilog_customer_app/core/features/home/model/notification_model.dart';
import 'package:ginilog_customer_app/core/features/home/model/riders_response_model.dart';
import 'package:http/http.dart' as http;

class HomeService {
  // Logistics Company
  Future<Map<String, dynamic>?> getPlaceDetails(String placeId) async {
    final String apiKey = "AIzaSyA1WkH5DbnyUVLhPtqo_qj3Bmr0uKPolSw";
    final String url =
        "https://maps.googleapis.com/maps/api/place/details/json?place_id=$placeId&key=$apiKey";

    final response = await http.get(Uri.parse(url));

    if (response.statusCode == 200) {
      final data = json.decode(response.body);
      printData('Places Details', data);
      return data['result'];
    } else {
      printData("Error fetching place details:", response.body);
      return null;
    }
  }

  Future<LogisticResponseModel> getLogisticsData(
      {required String stationId}) async {
    LogisticResponseModel data = LogisticResponseModel();
    try {
      Map<String, String> headers2 = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}'
      };
      var url = Uri.parse("${Endpoints.baseUrl}Logistics/$stationId");
      var response = await http.get(url, headers: headers2);
      if (response.statusCode == 200 || response.statusCode == 201) {
        final item = json.decode(response.body);
        data = LogisticResponseModel.fromJson(
            item); // Mapping json response to our data model
        printData('Station Details', data);
        return data;
      } else {
        printData('Station Details Error', response.body);
        return data;
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }

  Future<List<LogisticResponseModel>> getAllLogisticsData() async {
    List<LogisticResponseModel> data = [];
    try {
      Map<String, String> headers2 = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}'
      };
      var url = Uri.parse("${Endpoints.baseUrl}Logistics");
      var response = await http.get(url, headers: headers2);
      if (response.statusCode == 200 || response.statusCode == 201) {
        var data1 = logisticResponseModelFromJson(response.body);
        data = data1;
        printData('All Logistics Company', data);
        return data;
      } else {
        printData('All Logistics Company Error', response.body);
        return data;
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }

  Stream<List<LogisticResponseModel>> getAllStreamLogisticsData() async* {
    List<LogisticResponseModel> data = [];
    try {
      Map<String, String> headers2 = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}'
      };
      var url = Uri.parse("${Endpoints.baseUrl}Logistics");
      var response = await http.get(url, headers: headers2);
      if (response.statusCode == 200 || response.statusCode == 201) {
        var data1 = logisticResponseModelFromJson(response.body);
        data = data1;
        printData('All Logistics Company2S', response.body);
        yield data;
      } else {
        printData('All Logistics Company Error2S', response.body);
      }
    } catch (e) {
      printData('Error', e.toString());
      yield* Stream.error(e);
    }
  }

// Notification
  Future<bool> sendNotification({
    required String title,
    required String body,
    required String notificationType,
    required String deviceToken,
  }) async {
    try {
      var stingUrl = Uri.parse("${Endpoints.baseUrl}Notifications");
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
        'userId': globals.userId.toString(),
      };
      final msg = jsonEncode(
        {
          "title": title,
          "body": body,
          "deviceToken": deviceToken,
          "notificationType": notificationType,
        },
      );
      http.Response response =
          await http.post(stingUrl, body: msg, headers: headers);
      if (response.statusCode == 200 || response.statusCode == 201) {
        printData("Response", response.body);
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

  Future<bool> sendNotificationRider({
    required String title,
    required String body,
    required String notificationType,
    required String riderDeviceToken,
    required String riderId,
  }) async {
    try {
      var stingUrl = Uri.parse("${Endpoints.baseUrl}Notifications");
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
        'userId': riderId,
      };
      final msg = jsonEncode(
        {
          "title": title,
          "body": body,
          "deviceToken": riderDeviceToken,
          "notificationType": notificationType,
        },
      );
      http.Response response =
          await http.post(stingUrl, body: msg, headers: headers);
      if (response.statusCode == 200 || response.statusCode == 201) {
        printData("Response", response.body);
        return true;
      } else {
        printData("Error", response.body);
        printData("Error3", riderId);
        return false;
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }

  Future<NotificationResponseModel> getNotificationData(
      {required String id}) async {
    NotificationResponseModel data = NotificationResponseModel();
    try {
      Map<String, String> headers2 = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}'
      };
      var url = Uri.parse("${Endpoints.baseUrl}Notifications/$id");
      var response = await http.get(url, headers: headers2);
      if (response.statusCode == 200 || response.statusCode == 201) {
        final item = json.decode(response.body);
        data = NotificationResponseModel.fromJson(
            item); // Mapping json response to our data model
        printData('Notification Details', data);
        return data;
      } else {
        printData('Notification Details Error', response.body);
        return data;
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }

  Future<List<NotificationResponseModel>> getAllNotificationData() async {
    List<NotificationResponseModel> data = [];
    try {
      Map<String, String> headers2 = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
      };
      var url = Uri.parse(
        "${Endpoints.baseUrl}Notifications",
      );
      var response = await http.get(url, headers: headers2);
      if (response.statusCode == 200 || response.statusCode == 201) {
        var data1 = notificationResponseModelFromJson(response.body);
        data = data1;
        printData('All Notification', response.body);
        return data;
      } else {
        printData('All Notification Error', response.body);
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
    return data;
  }

// Riders
  Future<List<RidersResponseModel>> getAllRidersData() async {
    List<RidersResponseModel> data = [];
    try {
      Map<String, String> headers2 = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
      };
      var url = Uri.parse(
        "${Endpoints.baseUrl}Logistics/rider",
      );
      var response = await http.get(url, headers: headers2);
      if (response.statusCode == 200 || response.statusCode == 201) {
        var data1 = ridersResponseModelFromJson(response.body);
        data = data1;
        printData('All Riders', response.body);
        return data;
      } else {
        printData('All Riders Error', response.body);
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
    return data;
  }

  // Add Rider Review
  Future<http.Response> addRiderReview(
      {required String riderId,
      required String orderId,
      required String reviewMessage,
      required double ratingNum}) async {
    try {
      var stingUrl =
          Uri.parse("${Endpoints.baseUrl}Riders/update-rider-review/$riderId");
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
        "userId": "${globals.userId}",
      };
      final msg = jsonEncode(
        {
          "reviewMessage": reviewMessage,
          "ratingNum": ratingNum,
          "orderId": orderId
        },
      );
      http.Response response =
          await http.put(stingUrl, body: msg, headers: headers);
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

  // Add Logistics Company Review
  Future<http.Response> addLogisticsReview(
      {required String stationId,
      required String orderId,
      required String reviewMessage,
      required double ratingNum}) async {
    try {
      var stingUrl = Uri.parse(
          "${Endpoints.baseUrl}Logistics/update-gas-station-review/$stationId");
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
        "userId": "${globals.userId}",
      };
      final msg = jsonEncode(
        {
          "reviewMessage": reviewMessage,
          "ratingNum": ratingNum,
          "orderId": orderId
        },
      );
      http.Response response =
          await http.put(stingUrl, body: msg, headers: headers);
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
}
