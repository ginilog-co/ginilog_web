import 'dart:convert';

import 'package:ginilog_customer_app/core/components/extension/error_handling.dart';
import 'package:ginilog_customer_app/core/components/helpers/endpoints.dart';
import 'package:ginilog_customer_app/core/components/helpers/globals.dart';
import 'package:ginilog_customer_app/core/components/utils/constants.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/features/home/model/company_response_model.dart';
import 'package:ginilog_customer_app/core/features/home/model/notification_model.dart';
import 'package:ginilog_customer_app/core/features/home/model/riders_response_model.dart';
import 'package:http/http.dart' as http;

class HomeService {
  final cache = DefaultCacheManager();
  // Logistics Companys
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

  Future<List<LogisticResponseModel>> getAllLogisticsData2() async {
    List<LogisticResponseModel> data = [];
    try {
      Map<String, String> headers2 = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}'
      };
      var url = Uri.parse("${Endpoints.baseUrl}Logistics");
      final response =
          await cache.downloadFile(url.toString(), authHeaders: headers2);
      String file = await response.file.readAsString();
      if (response.file.existsSync()) {
        var data1 = logisticResponseModelFromJson(file);
        data = data1;
        printData('All Logistics Company', data);
        return data;
      } else {
        printData('All Logistics Company Error', file);
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
        'Authorization': 'Bearer ${globals.token}',
      };
      var url = Uri.parse("${Endpoints.baseUrl}Logistics");
      final response =
          await cache.downloadFile(url.toString(), authHeaders: headers2);

      if (response.file.existsSync()) {
        String file = await response.file.readAsString();

        // Validate file content
        if (file.isNotEmpty && (file.startsWith('{') || file.startsWith('['))) {
          try {
            var data1 = logisticResponseModelFromJson(file);
            data = data1;
            printData('All Logistics Company', file);
            return data;
          } catch (e) {
            printData('All Logistics Company Parsing Error', e.toString());
            return Future.error('Invalid JSON format');
          }
        } else {
          response.file.deleteSync();
          var data1 = logisticResponseModelFromJson(file);
          data = data1;
          printData('All Logistics Company', file);
          return data;
        }
      } else {
        printData('File not found', response.file.path);
        return [];
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
        'Authorization': 'Bearer ${globals.token}',
      };
      var url = Uri.parse("${Endpoints.baseUrl}Logistics");
      final response =
          await cache.downloadFile(url.toString(), authHeaders: headers2);

      if (response.file.existsSync()) {
        String file = await response.file.readAsString();

        // Validate file content
        if (file.isNotEmpty && (file.startsWith('{') || file.startsWith('['))) {
          try {
            var data1 = logisticResponseModelFromJson(file);
            data = data1;
            printData('All Logistics Company2S', file);
            yield data;
          } catch (e) {
            printData('All Logistics Company Parsing Error', e.toString());
            yield* Stream.error('Invalid JSON format');
          }
        } else {
          response.file.deleteSync();
          var data1 = logisticResponseModelFromJson(file);
          data = data1;
          printData('All Logistics Company2S', file);
          yield data;
        }
      } else {
        printData('File not found', response.file.path);
        yield [];
      }
    } catch (e) {
      printData('Error', e.toString());
      yield* Stream.error(handleHttpError(e));
    }
  }

  // Stream<List<LogisticResponseModel>> getAllStreamLogisticsData2(
  //     {required String state, required String city}) async* {
  //   List<LogisticResponseModel> data = [];
  //   try {
  //     Map<String, String> headers2 = {
  //       'Content-Type': 'application/json',
  //       'Accept': 'application/json',
  //       'Authorization': 'Bearer ${globals.token}'
  //     };
  //     var url = Uri.parse(
  //         "${Endpoints.baseUrl}Logistics/filter?State=$state&City=$city");
  //     final response =
  //         await cache.downloadFile(url.toString(), authHeaders: headers2);
  //     String file = await response.file.readAsString();
  //     if (response.file.existsSync()) {
  //       var data1 = logisticResponseModelFromJson(file);
  //       data = data1;
  //       printData('All Logistics Company2S', file);
  //       yield data;
  //     } else {
  //       printData('All Logistics Company Error2S', file);
  //     }
  //   } catch (e) {
  //     printData('Error', e.toString());
  //     yield* Stream.error(e);
  //   }
  // }

  Stream<List<LogisticResponseModel>> getAllStreamLogisticsData2(
      {required String state, required String city}) async* {
    List<LogisticResponseModel> data = [];
    try {
      Map<String, String> headers2 = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
      };
      var url = Uri.parse(
          "${Endpoints.baseUrl}Logistics/filter?State=$state&City=$city");
      final response =
          await cache.downloadFile(url.toString(), authHeaders: headers2);

      if (response.file.existsSync()) {
        String file = await response.file.readAsString();

        // Validate file content
        if (file.isNotEmpty && (file.startsWith('{') || file.startsWith('['))) {
          try {
            var data1 = logisticResponseModelFromJson(file);
            data = data1;
            printData('All Logistics Company2S', file);
            yield data;
          } catch (e) {
            printData('All Logistics Company Parsing Error', e.toString());
            yield* Stream.error('Invalid JSON format');
          }
        } else {
          response.file.deleteSync();
          var data1 = logisticResponseModelFromJson(file);
          data = data1;
          printData('All Logistics Company2S', file);
          yield data;
        }
      } else {
        printData('File not found', response.file.path);
        yield [];
      }
    } catch (e) {
      printData('Error', e.toString());
      yield* Stream.error(handleHttpError(e));
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
      var url = Uri.parse("${Endpoints.baseUrl}Notifications");
      final response =
          await cache.downloadFile(url.toString(), authHeaders: headers2);

      if (response.file.existsSync()) {
        String file = await response.file.readAsString();

        // Validate file content
        if (file.isNotEmpty && (file.startsWith('{') || file.startsWith('['))) {
          try {
            var data1 = notificationResponseModelFromJson(file);
            data = data1;
            printData('All Notification', file);
            return data;
          } catch (e) {
            printData('All Notification Parsing Error', e.toString());
            return Future.error('Invalid JSON format');
          }
        } else {
          response.file.deleteSync();
          var data1 = notificationResponseModelFromJson(file);
          data = data1;
          printData('All Notification', file);
          return data;
        }
      } else {
        printData('File not found', response.file.path);
        return [];
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
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
      var url = Uri.parse("${Endpoints.baseUrl}Logistics/rider");
      final response =
          await cache.downloadFile(url.toString(), authHeaders: headers2);

      if (response.file.existsSync()) {
        String file = await response.file.readAsString();

        // Validate file content
        if (file.isNotEmpty && (file.startsWith('{') || file.startsWith('['))) {
          try {
            var data1 = ridersResponseModelFromJson(file);
            data = data1;
            printData('All Riders', file);
            return data;
          } catch (e) {
            printData('All Riders Parsing Error', e.toString());
            return Future.error('Invalid JSON format');
          }
        } else {
          response.file.deleteSync();
          var data1 = ridersResponseModelFromJson(file);
          data = data1;
          printData('All Riders', file);
          return data;
        }
      } else {
        printData('File not found', response.file.path);
        return [];
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }

  // Add Rider Review
  Future<bool> addRiderReview(
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

  // Add Logistics Company Review
  Future<bool> addLogisticsReview(
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
