// ignore_for_file: use_build_context_synchronously

import 'package:ginilog_customer_app/core/components/utils/constants.dart';
import 'package:ginilog_customer_app/core/features/home/model/advert_response_model.dart';
import 'package:ginilog_customer_app/core/features/home/model/company_response_model.dart';
import 'package:ginilog_customer_app/core/features/home/model/notification_model.dart';
import 'package:ginilog_customer_app/core/features/home/services/home_service.dart';

import '../../../components/utils/package_export.dart';

abstract class HomeState {
  final bool hasLoadedInitially;
  final int visitCount;
  const HomeState({this.hasLoadedInitially = false, this.visitCount = 0});
}

class HomeInitial extends HomeState {
  const HomeInitial() : super();
}

class HomeLoading extends HomeState {
  const HomeLoading({super.hasLoadedInitially}) : super();
}

class HomeRefreshing extends HomeState {
  const HomeRefreshing() : super(hasLoadedInitially: true, visitCount: 0);
}

class HomeSuccess<T> extends HomeState {
  final String message;
  final T? data;
  final List<T>? listData;
  HomeSuccess({
    required this.message,
    this.data,
    this.listData,
    super.visitCount,
  }) : super(hasLoadedInitially: true);
}

class HomeUpdate extends HomeState {
  final String message;
  HomeUpdate({required this.message}) : super(hasLoadedInitially: true);
}

class HomeFailure extends HomeState {
  final String error;
  HomeFailure({required this.error}) : super(hasLoadedInitially: false);
}

class HomeNotifier extends StateNotifier<HomeState> {
  final HomeService home;
  // Logistics
  List<LogisticResponseModel> allLogisticss = [];
  bool _hasFetchedLogistics = false;
  int _visitLogisticsCounter = 0;

  // Notifications
  List<NotificationResponseModel> notifications = [];
  bool _hasFetchedNotifications = false;
  int _visitNotificationCounter = 0;

  // Advert

  List<AdvertResponseModel> advertisements = [];
  bool _hasFetchedAdvertisements = false;
  int _visitAdvertisementCounter = 0;

  HomeNotifier({required this.home}) : super(HomeInitial());

  getAllLogisticsData({bool forceRefresh = false}) async {
    _visitLogisticsCounter++;
    printData("Logistics Visit Count:", "$_visitLogisticsCounter");
    try {
      if (_visitLogisticsCounter == 1) {
        if (!_hasFetchedLogistics || forceRefresh) {
          if (!_hasFetchedLogistics) {
            state = const HomeLoading();
          } else {
            state = const HomeRefreshing();
          }

          final response = await home.getAllLogisticsData();
          allLogisticss = response;
          _hasFetchedLogistics = true;
          state = HomeSuccess<LogisticResponseModel>(
            message: "All Logistics Loaded",
            listData: allLogisticss,
            visitCount: _visitLogisticsCounter,
          );
        }
      } else {
        if (!mounted) {
          state = HomeLoading();
          return;
        }
        List<LogisticResponseModel> response2 =
            await home.getAllLogisticsData();
        allLogisticss = response2;
        state = HomeSuccess(message: "All Homes Loaded");
      }
    } on Exception catch (e) {
      state = HomeFailure(error: e.toString());
    }
  }

  LogisticResponseModel? fetchLogisticsById(String stationId) {
    LogisticResponseModel? logistics = LogisticResponseModel();
    for (var logistics2 in allLogisticss) {
      if (logistics2.id == stationId) {
        logistics = logistics2;
      }
    }
    return logistics;
  }

  getAllNotificationData({bool forceRefresh = false}) async {
    _visitNotificationCounter++;
    printData("Notification Visit Count:", "$_visitNotificationCounter");
    try {
      if (_visitNotificationCounter == 1) {
        if (!_hasFetchedNotifications || forceRefresh) {
          if (!_hasFetchedNotifications) {
            state = const HomeLoading();
          } else {
            state = const HomeRefreshing();
          }

          final response = await home.getAllNotificationData();
          notifications = response;
          _hasFetchedNotifications = true;
          state = HomeSuccess<NotificationResponseModel>(
            message: "All Notifications Loaded",
            listData: notifications,
            visitCount: _visitNotificationCounter,
          );
        }
      } else {
        if (!mounted) {
          state = HomeLoading();
          return;
        }
        List<NotificationResponseModel> response2 =
            await home.getAllNotificationData();
        notifications = response2;
        state = HomeSuccess(message: "All Notifications Loaded");
      }
    } on Exception catch (e) {
      state = HomeFailure(error: e.toString());
    }
  }

  //Advert
  getAllAdvertData({bool forceRefresh = false}) async {
    _visitAdvertisementCounter++;
    printData("Advertisement Visit Count:", "$_visitAdvertisementCounter");
    try {
      if (_visitAdvertisementCounter == 1) {
        if (!_hasFetchedAdvertisements || forceRefresh) {
          if (!_hasFetchedAdvertisements) {
            state = const HomeLoading();
          } else {
            state = const HomeRefreshing();
          }

          final response = await home.getAllAdvertisements();
          advertisements = response;
          _hasFetchedAdvertisements = true;
          state = HomeSuccess<AdvertResponseModel>(
            message: "All Advertisements Loaded",
            listData: advertisements,
            visitCount: _visitAdvertisementCounter,
          );
        }
      } else {
        if (!mounted) {
          state = HomeLoading();
          return;
        }
        List<AdvertResponseModel> response2 = await home.getAllAdvertisements();
        advertisements = response2;
        state = HomeSuccess(message: "All Advertisements Loaded");
      }
    } on Exception catch (e) {
      state = HomeFailure(error: e.toString());
    }
  }
}

final streamRepositoryProvider = Provider<HomeService>((ref) => HomeService());
final homeProvider = StateNotifierProvider<HomeNotifier, HomeState>((ref) {
  final HomeService home = HomeService();
  return HomeNotifier(home: home);
});
