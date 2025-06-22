// ignore_for_file: use_build_context_synchronously

import 'dart:convert';
import 'dart:async';

import 'package:ginilog_customer_app/core/components/utils/constants.dart';
import 'package:ginilog_customer_app/core/components/widgets/custom_snackbar.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/customer_book_response_model.dart';
import 'package:ginilog_customer_app/core/features/bookings/services/booking_services.dart';
import '../../../components/utils/package_export.dart';

abstract class CustomerBookedState {
  final bool isCreatingBooking;
  final bool hasLoadedInitially;
  final int visitCount;

  const CustomerBookedState({
    this.isCreatingBooking = false,
    this.hasLoadedInitially = false,
    this.visitCount = 0,
  });
}

class CustomerBookedStateInitial extends CustomerBookedState {
  const CustomerBookedStateInitial() : super();
}

class CustomerBookedStateLoading extends CustomerBookedState {
  const CustomerBookedStateLoading({super.hasLoadedInitially})
    : super(isCreatingBooking: true);
}

class CustomerBookedStateRefreshing extends CustomerBookedState {
  const CustomerBookedStateRefreshing()
    : super(isCreatingBooking: false, hasLoadedInitially: true);
}

class CustomerBookedStateSuccess<T> extends CustomerBookedState {
  final String message;
  final T? data;
  final List<T>? listData;

  const CustomerBookedStateSuccess({
    required this.message,
    this.data,
    this.listData,
    super.visitCount,
  }) : super(isCreatingBooking: false, hasLoadedInitially: true);
}

class CustomerBookedStateUpdate extends CustomerBookedState {
  final String message;
  const CustomerBookedStateUpdate({required this.message})
    : super(isCreatingBooking: false, hasLoadedInitially: true);
}

class CustomerBookedStateFailure extends CustomerBookedState {
  final String error;
  const CustomerBookedStateFailure({required this.error})
    : super(isCreatingBooking: false);
}

class CustomerBookedStateNotifier extends StateNotifier<CustomerBookedState> {
  final BookingsService booking;

  List<CustomerBookResponseModel> allCustomerReservations = [];
  CustomerBookResponseModel customerReservationModel =
      CustomerBookResponseModel();

  bool _hasFetchedReservations = false;
  final Map<String, bool> _fetchedAccomodationById = {};
  int _visitCounter = 0;

  CustomerBookedStateNotifier({required this.booking})
    : super(CustomerBookedStateInitial());

  Future<void> createBooking({
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
    required String paymentType,
    required BuildContext context,
  }) async {
    try {
      state = const CustomerBookedStateLoading();
      final response = await booking.bookAccomodationReservation(
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
        noOfDays: noOfDays,
        paymentType: paymentType,
      );

      if (response.statusCode == 200 || response.statusCode == 201) {
        final json = jsonDecode(response.body);
        final newBooking = CustomerBookResponseModel.fromJson(json);

        final index = allCustomerReservations.indexWhere(
          (x) => x.id == newBooking.id,
        );
        if (index != -1) {
          allCustomerReservations[index] = newBooking;
        } else {
          allCustomerReservations.insert(0, newBooking);
        }

        state = CustomerBookedStateSuccess<CustomerBookResponseModel>(
          message: "Booking Successfully",
          data: newBooking,
          listData: allCustomerReservations,
          visitCount: _visitCounter,
        );

        showCustomSnackbar(
          context,
          title: "Booking Addition",
          content: "Booking Added Successfully",
          type: SnackbarType.success,
          isTopPosition: false,
        );
      } else {
        state = CustomerBookedStateFailure(error: response.body);

        showCustomSnackbar(
          context,
          title: "Booking Addition",
          content: response.body,
          type: SnackbarType.error,
          isTopPosition: false,
        );
      }
    } catch (e) {
      state = CustomerBookedStateFailure(error: e.toString());
    }
  }

  Future<void> getCustomerReservationData({
    required String id,
    bool forceRefresh = false,
  }) async {
    try {
      if (!_fetchedAccomodationById.containsKey(id) || forceRefresh) {
        if (!_fetchedAccomodationById.containsKey(id)) {
          state = const CustomerBookedStateLoading();
        } else {
          state = const CustomerBookedStateRefreshing();
        }
        CustomerBookResponseModel response = await booking.getCustomerBookData(
          id: id,
        );
        customerReservationModel = response;
        _fetchedAccomodationById[id] = true;
        state = CustomerBookedStateSuccess<CustomerBookResponseModel>(
          message: "${response.id} Notification Loaded",
          data: customerReservationModel,
          visitCount: _visitCounter,
        );
      }
    } catch (e) {
      state = CustomerBookedStateFailure(error: e.toString());
    }
  }

  Future<void> getAllCustomerReservationData({bool refresh = false}) async {
    _visitCounter++;
    printData("CustomerState Visit Count:", "$_visitCounter");
    try {
      if (_visitCounter == 1) {
        if (!_hasFetchedReservations || refresh) {
          if (!_hasFetchedReservations) {
            state = const CustomerBookedStateLoading();
          } else {
            state = const CustomerBookedStateRefreshing();
          }
          final response = await booking.getAllCustomerBookData();
          allCustomerReservations = response;
          _hasFetchedReservations = true;
          state = CustomerBookedStateSuccess<CustomerBookResponseModel>(
            message: "All Accommodations Loaded",
            listData: response,
            visitCount: _visitCounter,
          );
        }
      } else {
        if (!mounted) {
          state = CustomerBookedStateLoading();
          return;
        }
        final response = await booking.getAllCustomerBookData();
        allCustomerReservations = response;
        _hasFetchedReservations = true;
        state = CustomerBookedStateSuccess<CustomerBookResponseModel>(
          message: "All Accommodations Loaded",
          listData: response,
          visitCount: _visitCounter,
        );
      }
    } catch (e) {
      state = CustomerBookedStateFailure(error: e.toString());
    }
  }
}

final streamRepositoryProvider = Provider<BookingsService>(
  (ref) => BookingsService(),
);

final customerBookedStateProvider =
    StateNotifierProvider<CustomerBookedStateNotifier, CustomerBookedState>((
      ref,
    ) {
      final bookingService = ref.read(streamRepositoryProvider);
      return CustomerBookedStateNotifier(booking: bookingService);
    });
