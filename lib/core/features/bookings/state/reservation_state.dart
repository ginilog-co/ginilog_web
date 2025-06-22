// ignore_for_file: use_build_context_synchronously

import 'package:ginilog_customer_app/core/components/utils/constants.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/accomodation_reservations_response_model.dart';
import 'package:ginilog_customer_app/core/features/bookings/services/booking_services.dart';
import '../../../components/utils/package_export.dart';

abstract class ReservationState {
  final bool isCreatingBooking;
  final bool hasLoadedInitially;
  final int visitCount;

  const ReservationState({
    this.isCreatingBooking = false,
    this.hasLoadedInitially = false,
    this.visitCount = 0,
  });
}

class ReservationStateInitial extends ReservationState {
  const ReservationStateInitial() : super();
}

class ReservationStateLoading extends ReservationState {
  const ReservationStateLoading({super.hasLoadedInitially})
    : super(isCreatingBooking: true);
}

class ReservationStateRefreshing extends ReservationState {
  const ReservationStateRefreshing()
    : super(isCreatingBooking: false, hasLoadedInitially: true);
}

class ReservationStateSuccess<T> extends ReservationState {
  final String message;
  final T? data;
  final List<T>? listData;

  const ReservationStateSuccess({
    required this.message,
    this.data,
    this.listData,
    super.visitCount,
  }) : super(isCreatingBooking: false, hasLoadedInitially: true);
}

class ReservationStateUpdate extends ReservationState {
  final String message;
  const ReservationStateUpdate({required this.message})
    : super(isCreatingBooking: false, hasLoadedInitially: true);
}

class ReservationStateFailure extends ReservationState {
  final String error;
  const ReservationStateFailure({required this.error})
    : super(isCreatingBooking: false);
}

class ReservationStateNotifier extends StateNotifier<ReservationState> {
  final BookingsService booking;

  // ACCOMODATION RESERVATIONS
  List<AccomodationReservationResponseModel> allAccomodationReservations = [];
  AccomodationReservationResponseModel reservationModel =
      AccomodationReservationResponseModel();

  bool _hasFetchedReservations = false;

  final Map<String, bool> _fetchedAccomodationById = {};
  int _visitCounter = 0;
  ReservationStateNotifier({required this.booking})
    : super(ReservationStateInitial());

  // ACCOMODATION RESERVATIONS

  Future<void> getAccomodationReservationData({
    required String id,
    bool forceRefresh = false,
  }) async {
    try {
      if (!_fetchedAccomodationById.containsKey(id) || forceRefresh) {
        if (!_fetchedAccomodationById.containsKey(id)) {
          state = const ReservationStateLoading();
        } else {
          state = const ReservationStateRefreshing();
        }
        AccomodationReservationResponseModel response = await booking
            .getAccomodationReservationData(id: id);
        reservationModel = response;
        _fetchedAccomodationById[id] = true;
        state = ReservationStateSuccess<AccomodationReservationResponseModel>(
          message: "${response.id} Notification Loaded",
          data: reservationModel,
        );
      }
    } catch (e) {
      state = ReservationStateFailure(error: e.toString());
    }
  }

  Future<void> getAllAccomodationReservationData({bool refresh = false}) async {
    _visitCounter++;
    printData("ReservationState Visit Count:", "$_visitCounter");

    try {
      if (_visitCounter == 1) {
        if (!_hasFetchedReservations || refresh) {
          if (!_hasFetchedReservations) {
            state = const ReservationStateLoading();
          } else {
            state = const ReservationStateRefreshing();
          }

          final response = await booking.getAllAccomodationReservationsData();
          allAccomodationReservations = response;
          _hasFetchedReservations = true;
          state = ReservationStateSuccess<AccomodationReservationResponseModel>(
            message: "All Accommodations Loaded",
            listData: response,
          );
        }
      } else {
        if (!mounted) {
          state = const ReservationStateLoading();
          return;
        }
        final response = await booking.getAllAccomodationReservationsData();
        allAccomodationReservations = response;
        _hasFetchedReservations = true;
        state = ReservationStateSuccess<AccomodationReservationResponseModel>(
          message: "All Accommodations Loaded",
          listData: response,
          visitCount: _visitCounter,
        );
      }
    } catch (e) {
      state = ReservationStateFailure(error: e.toString());
    }
  }
}

final streamRepositoryProvider = Provider<BookingsService>(
  (ref) => BookingsService(),
);

final reservationStateProvider =
    StateNotifierProvider<ReservationStateNotifier, ReservationState>((ref) {
      final bookingService = ref.read(streamRepositoryProvider);
      return ReservationStateNotifier(booking: bookingService);
    });
