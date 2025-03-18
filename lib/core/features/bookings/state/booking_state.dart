// ignore_for_file: use_build_context_synchronously

import 'package:ginilog_customer_app/core/components/widgets/custom_snackbar.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/accomodation_reservations_response_model.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/accomodation_response_model.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/customer_book_response_model.dart';
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

  // Accomodations
  List<AccomodationResponseModel> allAccomodations = [];
  AccomodationResponseModel accomodationModel = AccomodationResponseModel();

// ACCOMODATION RESERVATIONS
  List<AccomodationReservationResponseModel> allAccomodationReservations = [];
  List<CustomerBookResponseModel> allCustomerBooks = [];
  BookingNotifier({required this.booking}) : super(BookingInitial());

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

// ACCOMODATION RESERVATIONS
  createBooking(
      {required String reservationId,
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
      required BuildContext context}) async {
    try {
      if (!mounted) {
        state = BookingLoading();
        return;
      }
      var response = await booking.bookAccomodationReservation(
          reservationId: reservationId,
          customerName: customerName,
          customerPhoneNumber: customerPhoneNumber,
          customerEmail: customerEmail,
          trnxReference: trnxReference,
          paymentStatus: paymentStatus,
          numberOfGuests: numberOfGuests,
          comment: comment,
          paymentChannel: paymentChannel,
          reservationStartDate: reservationStartDate,
          reservationEndDate: reservationEndDate,
          noOfDays: noOfDays);
      if (response.statusCode == 200 || response.statusCode == 201) {
        state = BookingSuccess(message: "Booking Successfully");
        showCustomSnackbar(context,
            title: "Booking Addition",
            content: "Booking Added Successfully",
            type: SnackbarType.success,
            isTopPosition: false);
      } else {
        state = BookingFailure(error: response.body);
        showCustomSnackbar(context,
            title: "Booking Addition",
            content: response.body,
            type: SnackbarType.error,
            isTopPosition: false);
      }
    } on Exception catch (e) {
      state = BookingFailure(error: e.toString());
    }
  }

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

  getAllCustomerBookData() async {
    try {
      if (!mounted) {
        state = BookingLoading();
        return;
      }
      List<CustomerBookResponseModel> response2 =
          await booking.getAllCustomerBookData();
      allCustomerBooks = response2;
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
