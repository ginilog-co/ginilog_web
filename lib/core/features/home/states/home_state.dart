// ignore_for_file: use_build_context_synchronously

import 'package:ginilog_customer_app/core/components/extension/all_extension.dart';
import 'package:ginilog_customer_app/core/features/home/model/company_response_model.dart';
import 'package:ginilog_customer_app/core/features/home/model/notification_model.dart';
import 'package:ginilog_customer_app/core/features/home/model/riders_response_model.dart';
import 'package:ginilog_customer_app/core/features/home/services/home_service.dart';

import '../../../components/utils/package_export.dart';

abstract class HomeState {}

class HomeInitial extends HomeState {}

class HomeLoading extends HomeState {}

class HomeSuccess extends HomeState {
  final String message;
  HomeSuccess({required this.message});
}

class HomeUpdate extends HomeState {
  final String message;
  HomeUpdate({required this.message});
}

class HomeFailure extends HomeState {
  final String error;
  HomeFailure({required this.error});
}

class HomeNotifier extends StateNotifier<HomeState> {
  final HomeService home;
  List<LogisticResponseModel> allLogisticss = [];

  LogisticResponseModel logisticsModel = LogisticResponseModel();
  // Notifications
  List<NotificationResponseModel> notifications = [];
  NotificationResponseModel notificationModel = NotificationResponseModel();
  // Riders
  List<RidersResponseModel> allRiders = [];
  HomeNotifier({required this.home}) : super(HomeInitial());

  getLogisticsData({required String stationId}) async {
    try {
      if (!mounted) {
        state = HomeLoading();
        return;
      }
      LogisticResponseModel response2 =
          await home.getLogisticsData(stationId: stationId);
      logisticsModel = response2;
      state = HomeSuccess(message: "${response2.id} Home Loaded");
    } on Exception catch (e) {
      state = HomeFailure(error: e.toString());
    }
  }

  getAllLogisticsData() async {
    try {
      if (!mounted) {
        state = HomeLoading();
        return;
      }
      List<LogisticResponseModel> response2 = await home.getAllLogisticsData();
      allLogisticss = response2;
      state = HomeSuccess(message: "All Homes Loaded");
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

// Notifications
  getNotificationData({required String id}) async {
    try {
      if (!mounted) {
        state = HomeLoading();
        return;
      }
      NotificationResponseModel response2 =
          await home.getNotificationData(id: id);
      notificationModel = response2;
      state = HomeSuccess(message: "${response2.id} Notification Loaded");
    } on Exception catch (e) {
      state = HomeFailure(error: e.toString());
    }
  }

  getAllNotificationData() async {
    try {
      if (!mounted) {
        state = HomeLoading();
        return;
      }
      List<NotificationResponseModel> response2 =
          await home.getAllNotificationData();
      notifications = response2;
      state = HomeSuccess(message: "All Notifications Loaded");
    } on Exception catch (e) {
      state = HomeFailure(error: e.toString());
    }
  }

// Riders

  getAllRiderData() async {
    try {
      if (!mounted) {
        state = HomeLoading();
        return;
      }
      List<RidersResponseModel> response2 = await home.getAllRidersData();
      allRiders = response2;
      state = HomeSuccess(message: "All Riders Loaded");
    } on Exception catch (e) {
      state = HomeFailure(error: e.toString());
    }
  }

  RidersResponseModel? fetchRiderById(String riderId) {
    RidersResponseModel? rider = RidersResponseModel();
    for (var rider2 in allRiders) {
      if (rider2.id == riderId) {
        rider = rider2;
      }
    }
    return rider;
  }
}

final streamRepositoryProvider = Provider<HomeService>((ref) => HomeService());
final homeProvider = StateNotifierProvider<HomeNotifier, HomeState>((ref) {
  final HomeService home = HomeService();
  return HomeNotifier(home: home);
});

// New One Stream
class NewHomeStateService {
  late Stream<List<LogisticResponseModel>> _stream;
  logisticsInit() async {
    var homeService = HomeService();
    _stream = homeService.getAllStreamLogisticsData();
    final result = _stream;
    return result;
  }

  Stream<List<LogisticResponseModel>> get stream {
    return _stream;
  }
}

final streamRepo =
    Provider<NewHomeStateService>((ref) => NewHomeStateService());
final getListLogistics =
    StreamProvider.autoDispose<List<LogisticResponseModel>>((ref) {
  final streamServices = ref.watch(streamRepo);
  ref.refreshIn(const Duration(seconds: 1));
  streamServices.logisticsInit();
  return streamServices.stream;
});
