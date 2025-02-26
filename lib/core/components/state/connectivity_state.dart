import 'dart:async';

import 'package:ginilog_customer_app/core/components/utils/package_export.dart';

final streamProvider = StreamProvider<bool>((ref) {
  return Connectivity().onConnectivityChanged.map((event) {
    ConnectivityResult? result;
    for (var ev in event) {
      result = ev;
      ConnectivityUtility.checkConnectivity(ev);
    }
    return ConnectivityUtility.checkConnectivity(result!);
  });
});

class ConnectivityUtility {
  static bool checkConnectivity(ConnectivityResult result) {
    switch (result) {
      case ConnectivityResult.wifi:
      case ConnectivityResult.ethernet:
      case ConnectivityResult.mobile:
      case ConnectivityResult.vpn:
      case ConnectivityResult.other:
        return true;
      case ConnectivityResult.bluetooth:
      case ConnectivityResult.none:
        return false;
    }
  }
}

enum ConnectivityStatus { notDetermined, isConnected, isDisconnected }

class ConnectivityStatusNotifier extends StateNotifier<ConnectivityStatus> {
  StreamController<ConnectivityResult> controller =
      StreamController<ConnectivityResult>();

  ConnectivityStatus? lastResult;
  ConnectivityStatus? newState;

  ConnectivityStatusNotifier() : super(ConnectivityStatus.isConnected) {
    if (state == ConnectivityStatus.isConnected) {
      lastResult = ConnectivityStatus.isConnected;
    } else {
      lastResult = ConnectivityStatus.isDisconnected;
    }
    lastResult = ConnectivityStatus.notDetermined;
    Connectivity()
        .onConnectivityChanged
        .listen((List<ConnectivityResult> result) {
      if (result.contains(ConnectivityResult.mobile) ||
          result.contains(ConnectivityResult.wifi) ||
          result.contains(ConnectivityResult.ethernet) ||
          result.contains(ConnectivityResult.vpn)) {
        newState = ConnectivityStatus.isConnected;
      } else if (result.contains(ConnectivityResult.none)) {
        newState = ConnectivityStatus.isDisconnected;
      }

      if (newState != lastResult) {
        state = newState!;
        lastResult = newState;
      }
    });
  }
}

final connectivityStatusProviders = StateNotifierProvider((ref) {
  return ConnectivityStatusNotifier();
});
