import 'dart:convert';

import 'package:ginilog_customer_app/core/components/extension/error_handling.dart';
import 'package:ginilog_customer_app/core/components/helpers/endpoints.dart';
import 'package:ginilog_customer_app/core/components/helpers/globals.dart';
import 'package:ginilog_customer_app/core/components/utils/constants.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/accomodation_reservations_response_model.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/accomodation_response_model.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/customer_book_response_model.dart';
import 'package:http/http.dart' as http;

class BookingsService {
  final cache = DefaultCacheManager();

// Accomodation
  Future<AccomodationResponseModel> getAccomodationData(
      {required String accomodationId}) async {
    AccomodationResponseModel data = AccomodationResponseModel();
    try {
      Map<String, String> headers2 = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}'
      };
      var url = Uri.parse(
          "${Endpoints.baseUrl}Bookings/accomodation/$accomodationId");
      var response = await http.get(url, headers: headers2);
      if (response.statusCode == 200 || response.statusCode == 201) {
        final item = json.decode(response.body);
        data = AccomodationResponseModel.fromJson(
            item); // Mapping json response to our data model
        printData('Accomodation Details', data);
        return data;
      } else {
        printData('Accomodation Details Error', response.body);
        return data;
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }

  Future<List<AccomodationResponseModel>> getAllAccomodationData() async {
    List<AccomodationResponseModel> data = [];
    try {
      Map<String, String> headers2 = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
      };
      var url = Uri.parse("${Endpoints.baseUrl}Bookings/accomodation");
      final response =
          await cache.downloadFile(url.toString(), authHeaders: headers2);

      if (response.file.existsSync()) {
        String file = await response.file.readAsString();

        // Validate file content
        if (file.isNotEmpty && (file.startsWith('{') || file.startsWith('['))) {
          try {
            var data1 = accomodationResponseModelFromJson(file);
            data = data1;
            printData('All Accomodation', file);
            return data;
          } catch (e) {
            printData('All Accomodation Parsing Error', e.toString());
            return Future.error('Invalid JSON format');
          }
        } else {
          response.file.deleteSync();
          var data1 = accomodationResponseModelFromJson(file);
          data = data1;
          printData('All Accomodation', file);
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

  // Stream Accomodation

  Stream<List<AccomodationResponseModel>>
      getAllStreamAccomodationData() async* {
    List<AccomodationResponseModel> data = [];
    try {
      Map<String, String> headers2 = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
      };
      var url = Uri.parse("${Endpoints.baseUrl}Bookings/accomodation");
      final response =
          await cache.downloadFile(url.toString(), authHeaders: headers2);

      if (response.file.existsSync()) {
        String file = await response.file.readAsString();

        // Validate file content
        if (file.isNotEmpty && (file.startsWith('{') || file.startsWith('['))) {
          try {
            var data1 = accomodationResponseModelFromJson(file);
            data = data1;
            printData('All Accomodation Stream', file);
            yield data;
          } catch (e) {
            printData('All Accomodation Stream Parsing Error', e.toString());
            yield* Stream.error('Invalid JSON format');
          }
        } else {
          response.file.deleteSync();
          var data1 = accomodationResponseModelFromJson(file);
          data = data1;
          printData('All Accomodation Stream', file);
          yield data;
        }
      } else {
        printData('Accomodation Stream File not found', response.file.path);
        yield [];
      }
    } catch (e) {
      printData('Error', e.toString());
      yield* Stream.error(handleHttpError(e));
    }
  }

// ACCOMODATION RESERVATIONS
  Future<http.Response> bookAccomodationReservation({
    required String reservationId,
    required String customerName,
    required String customerPhoneNumber,
    required String customerEmail,
    required String trnxReference,
    required bool paymentStatus,
    required int numberOfGuests,
    required String comment,
    required String paymentChannel,
    required String reservationStartDate,
    required String reservationEndDate,
    required int noOfDays,
  }) async {
    try {
      var stingUrl = Uri.parse(
          "${Endpoints.baseUrl}Bookings/accomodation-reservations-customer");
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
        "reservationId": reservationId,
      };
      final msg = jsonEncode(
        {
          "customerName": customerName,
          "customerPhoneNumber": customerPhoneNumber,
          "customerEmail": customerEmail,
          "trnxReference": trnxReference,
          "paymentStatus": paymentStatus,
          "paymentChannel": paymentChannel,
          "numberOfGuests": numberOfGuests,
          "comment": comment,
          "ticketClosed": true,
          "reservationStartDate": reservationStartDate,
          "reservationEndDate": reservationEndDate,
          "noOfDays": noOfDays
        },
      );
      http.Response response =
          await http.post(stingUrl, body: msg, headers: headers);
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

  Future<List<AccomodationReservationResponseModel>>
      getAllAccomodationReservationsData() async {
    List<AccomodationReservationResponseModel> data = [];
    try {
      Map<String, String> headers2 = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
      };
      var url =
          Uri.parse("${Endpoints.baseUrl}Bookings/accomodation-reservations");
      final response =
          await cache.downloadFile(url.toString(), authHeaders: headers2);

      if (response.file.existsSync()) {
        String file = await response.file.readAsString();

        // Validate file content
        if (file.isNotEmpty && (file.startsWith('{') || file.startsWith('['))) {
          try {
            var data1 = accomodationReservationResponseModelFromJson(file);
            data = data1;
            printData('All Accomodation Reservations', file);
            return data;
          } catch (e) {
            printData(
                'All Accomodation Reservations Parsing Error', e.toString());
            return Future.error('Invalid JSON format');
          }
        } else {
          response.file.deleteSync();
          var data1 = accomodationReservationResponseModelFromJson(file);
          data = data1;
          printData('All Accomodation Reservations', file);
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

  Stream<List<AccomodationReservationResponseModel>>
      getAllStreamAccomodationReservationsData() async* {
    List<AccomodationReservationResponseModel> data = [];
    try {
      Map<String, String> headers2 = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
      };
      var url =
          Uri.parse("${Endpoints.baseUrl}Bookings/accomodation-reservations");
      final response =
          await cache.downloadFile(url.toString(), authHeaders: headers2);

      if (response.file.existsSync()) {
        String file = await response.file.readAsString();

        // Validate file content
        if (file.isNotEmpty && (file.startsWith('{') || file.startsWith('['))) {
          try {
            var data1 = accomodationReservationResponseModelFromJson(file);
            data = data1;
            printData('All Accomodation Reservations', file);
            yield data;
          } catch (e) {
            printData(
                'All Accomodation Reservations Parsing Error', e.toString());
            yield* Stream.error('Invalid JSON format');
          }
        } else {
          response.file.deleteSync();
          var data1 = accomodationReservationResponseModelFromJson(file);
          data = data1;
          printData('All Accomodation Reservations', file);
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

  Future<List<CustomerBookResponseModel>> getAllCustomerBookData() async {
    List<CustomerBookResponseModel> data = [];
    try {
      Map<String, String> headers2 = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
      };
      var url = Uri.parse(
          "${Endpoints.baseUrl}Bookings/accomodation-reservations-customer");
      final response =
          await cache.downloadFile(url.toString(), authHeaders: headers2);

      if (response.file.existsSync()) {
        String file = await response.file.readAsString();

        // Validate file content
        if (file.isNotEmpty && (file.startsWith('{') || file.startsWith('['))) {
          try {
            var data1 = customerBookResponseModelFromJson(file);
            data = data1;
            printData('All Customer Reservations', file);
            return data;
          } catch (e) {
            printData('All Customer Reservations Parsing Error', e.toString());
            return Future.error('Invalid JSON format');
          }
        } else {
          response.file.deleteSync();
          var data1 = customerBookResponseModelFromJson(file);
          data = data1;
          printData('All Customer Reservations', file);
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
}
