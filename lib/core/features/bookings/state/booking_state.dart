// ignore_for_file: use_build_context_synchronously

import 'package:ginilog_customer_app/core/components/utils/constants.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/accomodation_response_model.dart';
import 'package:ginilog_customer_app/core/features/bookings/services/booking_services.dart';
import '../../../components/utils/package_export.dart';

abstract class Bookings {
  final bool isCreatingBooking;
  final bool hasLoadedInitially;
  final int visitCount;

  const Bookings({
    this.isCreatingBooking = false,
    this.hasLoadedInitially = false,
    this.visitCount = 0,
  });
}

class BookingInitial extends Bookings {
  const BookingInitial() : super();
}

class BookingLoading extends Bookings {
  const BookingLoading({super.hasLoadedInitially})
    : super(isCreatingBooking: true);
}

class BookingRefreshing extends Bookings {
  const BookingRefreshing()
    : super(isCreatingBooking: false, hasLoadedInitially: true);
}

class BookingSuccess<T> extends Bookings {
  final String message;
  final T? data;
  final List<T>? listData;

  const BookingSuccess({
    required this.message,
    this.data,
    this.listData,
    super.visitCount,
  }) : super(isCreatingBooking: false, hasLoadedInitially: true);
}

class BookingUpdate extends Bookings {
  final String message;
  const BookingUpdate({required this.message})
    : super(isCreatingBooking: false, hasLoadedInitially: true);
}

class BookingFailure extends Bookings {
  final String error;
  const BookingFailure({required this.error}) : super(isCreatingBooking: false);
}

class BookingNotifier extends StateNotifier<Bookings> {
  final BookingsService booking;

  // Accomodations
  List<AccomodationResponseModel> allAccomodations = [];
  AccomodationResponseModel accomodationModel = AccomodationResponseModel();
  bool _hasFetchedAccomodations = false;

  final Map<String, bool> _fetchedAccomodationById = {};
  int _visitCounter = 0;
  BookingNotifier({required this.booking}) : super(BookingInitial());

  // Accomodation

  Future<void> getAccomodationData({
    required String id,
    bool forceRefresh = false,
  }) async {
    try {
      if (!_fetchedAccomodationById.containsKey(id) || forceRefresh) {
        if (!_fetchedAccomodationById.containsKey(id)) {
          state = const BookingLoading();
        } else {
          state = const BookingRefreshing();
        }
        AccomodationResponseModel response2 = await booking.getAccomodationData(
          accomodationId: id,
        );
        accomodationModel = response2;
        _fetchedAccomodationById[id] = true;
        state = BookingSuccess<AccomodationResponseModel>(
          message: "${response2.id} Notification Loaded",
          data: response2,
        );
      }
    } catch (e) {
      state = BookingFailure(error: e.toString());
    }
  }

  Future<void> getAllAccomodationData({bool forceRefresh = false}) async {
    _visitCounter++;
    printData("AllAccomodation Visit Count:", "$_visitCounter");
    try {
      if (_visitCounter == 1) {
        if (!_hasFetchedAccomodations || forceRefresh) {
          if (!_hasFetchedAccomodations) {
            state = const BookingLoading();
          } else {
            state = const BookingRefreshing();
          }

          final response = await booking.getAllAccomodationData();
          allAccomodations = response;
          _hasFetchedAccomodations = true;
          state = BookingSuccess<AccomodationResponseModel>(
            message: "All Accommodations Loaded",
            listData: response,
            visitCount: _visitCounter,
          );
        }
      } else {
        if (!mounted) {
          state = BookingLoading();
          return;
        }
        final response = await booking.getAllAccomodationData();
        allAccomodations = response;
        _hasFetchedAccomodations = true;
        state = BookingSuccess<AccomodationResponseModel>(
          message: "All Accommodations Loaded",
          listData: response,
          visitCount: _visitCounter,
        );
      }
    } catch (e) {
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
}

final streamRepositoryProvider = Provider<BookingsService>(
  (ref) => BookingsService(),
);

final bookingProvider = StateNotifierProvider<BookingNotifier, Bookings>((ref) {
  final bookingService = ref.read(streamRepositoryProvider);
  return BookingNotifier(booking: bookingService);
});
