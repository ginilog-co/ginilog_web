import 'dart:convert';

import 'package:ginilog_customer_app/core/components/extension/error_handling.dart';
import 'package:ginilog_customer_app/core/components/helpers/endpoints.dart';
import 'package:ginilog_customer_app/core/components/helpers/globals.dart';
import 'package:ginilog_customer_app/core/components/utils/constants.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/airline_response_model.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/flight_ticket_response_model.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/accomodation_reservations_response_model.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/accomodation_response_model.dart';
import 'package:http/http.dart' as http;

class BookingsService {
  final cache = DefaultCacheManager();
  // Package Airlines
  Future<bool> createAirline(
      {required String companyId,
      required num packageInputkg,
      required String trnxReference,
      required bool paymentStatus,
      required num distance,
      required int numberOfCylinders,
      required String addressId,
      required String paymentChannel}) async {
    try {
      var stingUrl = Uri.parse("${Endpoints.baseUrl}Bookings/airline");
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
        "companyId": companyId,
      };
      final msg = jsonEncode(
        {
          "trnxReference": trnxReference,
          "paymentStatus": paymentStatus,
          "distance": distance,
          "numberOfCylinders": numberOfCylinders,
          "paymentChannel": paymentChannel
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

  Future<AirlineResponseModel> getAirlineData(
      {required String airlineId}) async {
    AirlineResponseModel data = AirlineResponseModel();
    try {
      Map<String, String> headers2 = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}'
      };
      var url = Uri.parse("${Endpoints.baseUrl}Bookings/airline/$airlineId");
      var response = await http.get(url, headers: headers2);
      if (response.statusCode == 200 || response.statusCode == 201) {
        final item = json.decode(response.body);
        data = AirlineResponseModel.fromJson(
            item); // Mapping json response to our data model
        printData('Airline Details', data);
        return data;
      } else {
        printData('Airline Details Error', response.body);
        return data;
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }

  Future<List<AirlineResponseModel>> getAllAirlineData() async {
    List<AirlineResponseModel> data = [];
    try {
      Map<String, String> headers2 = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
      };
      var url = Uri.parse("${Endpoints.baseUrl}Bookings/airline");
      final response =
          await cache.downloadFile(url.toString(), authHeaders: headers2);

      if (response.file.existsSync()) {
        String file = await response.file.readAsString();

        // Validate file content
        if (file.isNotEmpty && (file.startsWith('{') || file.startsWith('['))) {
          try {
            var data1 = airlineResponseModelFromJson(file);
            data = data1;
            printData('All Airline Airline', file);
            return data;
          } catch (e) {
            printData('All Airline Airline Parsing Error', e.toString());
            return Future.error('Invalid JSON format');
          }
        } else {
          response.file.deleteSync();
          var data1 = airlineResponseModelFromJson(file);
          data = data1;
          printData('All Airline Airline', file);
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

  // Stream Package Airline

  Stream<List<AirlineResponseModel>> getAllStreamAirlineData() async* {
    List<AirlineResponseModel> data = [];
    try {
      Map<String, String> headers2 = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
      };
      var url = Uri.parse("${Endpoints.baseUrl}Bookings/airline");
      final response =
          await cache.downloadFile(url.toString(), authHeaders: headers2);

      if (response.file.existsSync()) {
        String file = await response.file.readAsString();

        // Validate file content
        if (file.isNotEmpty && (file.startsWith('{') || file.startsWith('['))) {
          try {
            var data1 = airlineResponseModelFromJson(file);
            data = data1;
            printData('All Airline Airline Stream', file);
            yield data;
          } catch (e) {
            printData('All Airline Airline Stream Parsing Error', e.toString());
            yield* Stream.error('Invalid JSON format');
          }
        } else {
          response.file.deleteSync();
          var data1 = airlineResponseModelFromJson(file);
          data = data1;
          printData('All Airline Airline Stream', file);
          yield data;
        }
      } else {
        printData('Package airline Stream File not found', response.file.path);
        yield [];
      }
    } catch (e) {
      printData('Error', e.toString());
      yield* Stream.error(handleHttpError(e));
    }
  }

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

// FLIGHT TICKET
  Future<FlightTicketResponseModel> getFlightTicketData(
      {required String flightTicketId}) async {
    FlightTicketResponseModel data = FlightTicketResponseModel();
    try {
      Map<String, String> headers2 = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}'
      };
      var url = Uri.parse(
          "${Endpoints.baseUrl}Bookings/airline-flight-ticket/$flightTicketId");
      var response = await http.get(url, headers: headers2);
      if (response.statusCode == 200 || response.statusCode == 201) {
        final item = json.decode(response.body);
        data = FlightTicketResponseModel.fromJson(
            item); // Mapping json response to our data model
        printData('FlightTicket Details', data);
        return data;
      } else {
        printData('FlightTicket Details Error', response.body);
        return data;
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }

  Future<List<FlightTicketResponseModel>> getAllFlightTicketData() async {
    List<FlightTicketResponseModel> data = [];
    try {
      Map<String, String> headers2 = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
      };
      var url = Uri.parse("${Endpoints.baseUrl}Bookings/airline-flight-ticket");
      final response =
          await cache.downloadFile(url.toString(), authHeaders: headers2);

      if (response.file.existsSync()) {
        String file = await response.file.readAsString();

        // Validate file content
        if (file.isNotEmpty && (file.startsWith('{') || file.startsWith('['))) {
          try {
            var data1 = flightTicketResponseModelFromJson(file);
            data = data1;
            printData('All FlightTicket', file);
            return data;
          } catch (e) {
            printData('All FlightTicket Parsing Error', e.toString());
            return Future.error('Invalid JSON format');
          }
        } else {
          response.file.deleteSync();
          var data1 = flightTicketResponseModelFromJson(file);
          data = data1;
          printData('All FlightTicket', file);
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

  // Stream FlightTicket

  Stream<List<FlightTicketResponseModel>>
      getAllStreamFlightTicketData() async* {
    List<FlightTicketResponseModel> data = [];
    try {
      Map<String, String> headers2 = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
      };
      var url = Uri.parse(
          "${Endpoints.baseUrl}Bookings/airline-flight-ticket?AnyItem=${globals.userId}");
      final response =
          await cache.downloadFile(url.toString(), authHeaders: headers2);

      if (response.file.existsSync()) {
        String file = await response.file.readAsString();

        // Validate file content
        if (file.isNotEmpty && (file.startsWith('{') || file.startsWith('['))) {
          try {
            var data1 = flightTicketResponseModelFromJson(file);
            data = data1;
            printData('All FlightTicket Stream', file);
            yield data;
          } catch (e) {
            printData('All FlightTicket Stream Parsing Error', e.toString());
            yield* Stream.error('Invalid JSON format');
          }
        } else {
          response.file.deleteSync();
          var data1 = flightTicketResponseModelFromJson(file);
          data = data1;
          printData('All FlightTicket Stream', file);
          yield data;
        }
      } else {
        printData('FlightTicket Stream File not found', response.file.path);
        yield [];
      }
    } catch (e) {
      printData('Error', e.toString());
      yield* Stream.error(handleHttpError(e));
    }
  }

// ACCOMODATION RESERVATIONS
  Future<bool> bookAccomodationReservation({
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
}
