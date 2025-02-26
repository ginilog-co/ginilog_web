// ignore_for_file: use_build_context_synchronously

import 'package:ginilog_customer_app/core/components/extension/all_extension.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/airline_response_model.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/flight_ticket_response_model.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/accomodation_reservations_response_model.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/accomodation_response_model.dart';
import 'package:ginilog_customer_app/core/features/bookings/services/booking_services.dart';
import '../../../components/utils/package_export.dart';

abstract class Bookings {}

class BookingInitial extends Bookings {}

class BookingLoading extends Bookings {}

class BookingSuccess extends Bookings {
  final String message;
  BookingSuccess({required this.message});
}

class BookingUpdate extends Bookings {
  final String message;
  BookingUpdate({required this.message});
}

class BookingFailure extends Bookings {
  final String error;
  BookingFailure({required this.error});
}

class BookingNotifier extends StateNotifier<Bookings> {
  final BookingsService booking;
  List<AirlineResponseModel> allAirline = [];

  AirlineResponseModel airLineModel = AirlineResponseModel();

  // Accomodations
  List<AccomodationResponseModel> allAccomodations = [];
  AccomodationResponseModel accomodationModel = AccomodationResponseModel();
  // FLIGHT TICKET
  List<FlightTicketResponseModel> allFlightTickets = [];
  FlightTicketResponseModel flightTicketModel = FlightTicketResponseModel();

// ACCOMODATION RESERVATIONS
  List<AccomodationReservationResponseModel> allAccomodationReservations = [];
  BookingNotifier({required this.booking}) : super(BookingInitial());

//Airline
  getAirlineData({required String airlineId}) async {
    try {
      if (!mounted) {
        state = BookingLoading();
        return;
      }
      AirlineResponseModel response2 =
          await booking.getAirlineData(airlineId: airlineId);
      airLineModel = response2;
      state = BookingSuccess(message: "${response2.id} Booking Loaded");
    } on Exception catch (e) {
      state = BookingFailure(error: e.toString());
    }
  }

  getAllAirlineData() async {
    try {
      if (!mounted) {
        state = BookingLoading();
        return;
      }
      List<AirlineResponseModel> response2 = await booking.getAllAirlineData();
      allAirline = response2;
      state = BookingSuccess(message: "All Bookings Loaded");
    } on Exception catch (e) {
      state = BookingFailure(error: e.toString());
    }
  }

  AirlineResponseModel? fetchAirlineById(String airlineId) {
    AirlineResponseModel? airline = AirlineResponseModel();
    for (var airline2 in allAirline) {
      if (airline2.id == airlineId) {
        airline = airline2;
      }
    }
    return airline;
  }

// Accomodation
  getAccomodationData({required String id}) async {
    try {
      if (!mounted) {
        state = BookingLoading();
        return;
      }
      AccomodationResponseModel response2 =
          await booking.getAccomodationData(accomodationId: id);
      accomodationModel = response2;
      state = BookingSuccess(message: "${response2.id} Notification Loaded");
    } on Exception catch (e) {
      state = BookingFailure(error: e.toString());
    }
  }

  getAllAccomodationData() async {
    try {
      if (!mounted) {
        state = BookingLoading();
        return;
      }
      List<AccomodationResponseModel> response2 =
          await booking.getAllAccomodationData();
      allAccomodations = response2;
      state = BookingSuccess(message: "All Accomodations Loaded");
    } on Exception catch (e) {
      state = BookingFailure(error: e.toString());
    }
  }

  AccomodationResponseModel? fetchAccomodationById(String accomodationId) {
    AccomodationResponseModel? accomodation = AccomodationResponseModel();
    for (var accomodation2 in allAccomodations) {
      if (accomodation2.id == accomodationId) {
        accomodation = accomodation2;
      }
    }
    return accomodation;
  }

// FLIGHT TICKET
  getFlightTicketData({required String id}) async {
    try {
      if (!mounted) {
        state = BookingLoading();
        return;
      }
      FlightTicketResponseModel response2 =
          await booking.getFlightTicketData(flightTicketId: id);
      flightTicketModel = response2;
      state = BookingSuccess(message: "${response2.id} Notification Loaded");
    } on Exception catch (e) {
      state = BookingFailure(error: e.toString());
    }
  }

  getAllFlightTicketData() async {
    try {
      if (!mounted) {
        state = BookingLoading();
        return;
      }
      List<FlightTicketResponseModel> response2 =
          await booking.getAllFlightTicketData();
      allFlightTickets = response2;
      state = BookingSuccess(message: "All Accomodations Loaded");
    } on Exception catch (e) {
      state = BookingFailure(error: e.toString());
    }
  }

  FlightTicketResponseModel? fetchFlightTicketById(String flightTicketId) {
    FlightTicketResponseModel? flightTicket = FlightTicketResponseModel();
    for (var flightTicket2 in allFlightTickets) {
      if (flightTicket2.id == flightTicketId) {
        flightTicket = flightTicket2;
      }
    }
    return flightTicket;
  }

// ACCOMODATION RESERVATIONS

  getAllAccomodationReservationData() async {
    try {
      if (!mounted) {
        state = BookingLoading();
        return;
      }
      List<AccomodationReservationResponseModel> response2 =
          await booking.getAllAccomodationReservationsData();
      allAccomodationReservations = response2;
      state = BookingSuccess(message: "All Accomodations Loaded");
    } on Exception catch (e) {
      state = BookingFailure(error: e.toString());
    }
  }
}

final streamRepositoryProvider =
    Provider<BookingsService>((ref) => BookingsService());
final bookingProvider = StateNotifierProvider<BookingNotifier, Bookings>((ref) {
  final BookingsService booking = BookingsService();
  return BookingNotifier(booking: booking);
});

// New One Stream
class NewBookingsService {
  late Stream<List<AirlineResponseModel>> _stream;
  airlineInit() async {
    var bookingsService = BookingsService();
    _stream = bookingsService.getAllStreamAirlineData();
    final result = _stream;
    return result;
  }

  Stream<List<AirlineResponseModel>> get stream {
    return _stream;
  }
}

final streamRepo = Provider<NewBookingsService>((ref) => NewBookingsService());
final getListAirline =
    StreamProvider.autoDispose<List<AirlineResponseModel>>((ref) {
  final streamServices = ref.watch(streamRepo);
  ref.refreshIn(const Duration(seconds: 1));
  streamServices.airlineInit();
  return streamServices.stream;
});
