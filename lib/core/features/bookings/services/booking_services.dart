import 'dart:convert';

import 'package:ginilog_customer_app/core/components/extension/error_handling.dart';
import 'package:ginilog_customer_app/core/components/helpers/endpoints.dart';
import 'package:ginilog_customer_app/core/components/helpers/globals.dart';
import 'package:ginilog_customer_app/core/components/utils/constants.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/accomodation_reservations_response_model.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/accomodation_response_model.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/customer_book_response_model.dart';
import 'package:http/http.dart' as http;

class BookingsService {
// Accomodation
  Future<List<AccomodationResponseModel>> getAllAccomodationData() async {
    List<AccomodationResponseModel> data = [];
    try {
      Map<String, String> headers2 = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
      };
      var url = Uri.parse(
        "${Endpoints.baseUrl}Bookings/accomodation",
      );
      var response = await http.get(url, headers: headers2);
      if (response.statusCode == 200 || response.statusCode == 201) {
        var data1 = accomodationResponseModelFromJson(response.body);
        data = data1;
        printData('All Accomodation', response.body);
        return data;
      } else {
        printData('All Accomodation Error', response.body);
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
    return data;
  }

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
      var url = Uri.parse(
        "${Endpoints.baseUrl}Bookings/accomodation-reservations",
      );
      var response = await http.get(url, headers: headers2);
      if (response.statusCode == 200 || response.statusCode == 201) {
        var data1 = accomodationReservationResponseModelFromJson(response.body);
        data = data1;
        printData('All Accomodation Reservations', response.body);
        return data;
      } else {
        printData('All Accomodation Reservations Error', response.body);
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
    return data;
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
        "${Endpoints.baseUrl}Bookings/accomodation-reservations-customer",
      );
      var response = await http.get(url, headers: headers2);
      if (response.statusCode == 200 || response.statusCode == 201) {
        var data1 = customerBookResponseModelFromJson(response.body);
        data = data1;
        printData('All Customer Reservations', response.body);
        return data;
      } else {
        printData('All Customer Reservations Error', response.body);
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
    return data;
  }

  Future<http.Response> addAccomodationReview(
      {required String accomodationId,
      required String reviewMessage,
      required double ratingNum}) async {
    try {
      var stingUrl = Uri.parse(
          "${Endpoints.baseUrl}Bookings/update-accomodation-review/$accomodationId");
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}'
      };
      final msg = jsonEncode(
        {"reviewMessage": reviewMessage, "ratingNum": ratingNum},
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
