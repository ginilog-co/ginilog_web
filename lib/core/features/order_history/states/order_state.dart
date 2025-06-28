// ignore_for_file: use_build_context_synchronously

import 'dart:convert';

import 'package:ginilog_customer_app/core/components/extension/all_extension.dart';
import 'package:ginilog_customer_app/core/components/utils/constants.dart';
import 'package:ginilog_customer_app/core/features/order_history/model/package_orders_model.dart';
import 'package:ginilog_customer_app/core/features/order_history/services/package_order_service.dart';

import '../../../components/utils/package_export.dart';

abstract class PackageOrderState {
  final bool isCreatingOrder;
  final bool hasLoadedInitially;
  final int visitCount;
  const PackageOrderState({
    this.isCreatingOrder = false,
    this.hasLoadedInitially = false,
    this.visitCount = 0,
  });
}

class PackageOrderInitial extends PackageOrderState {
  const PackageOrderInitial() : super();
}

class PackageOrderLoading extends PackageOrderState {
  const PackageOrderLoading({super.hasLoadedInitially})
    : super(isCreatingOrder: true);
}

class PackageOrderRefreshing extends PackageOrderState {
  const PackageOrderRefreshing()
    : super(isCreatingOrder: false, hasLoadedInitially: true);
}

class PackageOrderSuccess<T> extends PackageOrderState {
  final String message;
  final T? data;
  final List<T>? listData;
  PackageOrderSuccess({
    required this.message,
    this.data,
    this.listData,
    super.visitCount,
  }) : super(isCreatingOrder: false, hasLoadedInitially: true);
}

class PackageOrderUpdate extends PackageOrderState {
  final String message;
  PackageOrderUpdate({required this.message})
    : super(isCreatingOrder: false, hasLoadedInitially: true);
}

class PackageOrderFailure extends PackageOrderState {
  final String error;
  PackageOrderFailure({required this.error})
    : super(isCreatingOrder: false, hasLoadedInitially: false);
}

class PackageOrderNotifier extends StateNotifier<PackageOrderState> {
  final PackageOrderService packageOrder;
  List<PackageOrderResponseModel> allPackageOrders = [];

  PackageOrderResponseModel? packageOrderModel;
  // Notifications
  bool _hasFetchedOrders = false;
  final Map<String, bool> _fetchedOrdersById = {};
  int _visitCounter = 0;
  // Websocket
  WebSocketChannel? _channel;
  bool isConnected = false;
  int reconnectionAttempts = 0;
  String orderId = "";
  PackageOrderNotifier({required this.packageOrder})
    : super(PackageOrderInitial());

  bool _isMounted = true;

  @override
  void dispose() {
    _isMounted = false;
    disconnect();
    super.dispose();
  }

  void setInitialOrder(PackageOrderResponseModel order) {
    packageOrderModel = order;

    if (_isMounted) {
      state = PackageOrderSuccess<PackageOrderResponseModel>(
        message: "Tracking started",
        data: order,
      );
    }
  }

  // Initialize WebSocket connection
  Future<void> connectAndJoinOrder({
    required String orderId,
    required bool isSingle,
  }) async {
    this.orderId = orderId;
    _connectWebSocket(isSingle: isSingle);
  }

  void _connectWebSocket({required bool isSingle}) {
    try {
      _channel = WebSocketChannel.connect(
        Uri.parse('ws://api-data.ginilog.com/ws'),
      );
      printData("Initiate Connection:", '✅ Connected to WebSocket');
      isConnected = true;
      reconnectionAttempts = 0;

      final joinMessage =
          '{"action": "JoinOrderTracking", "orderId": "$orderId"}';
      _channel!.sink.add(joinMessage);
      printData("Connection Established:", 'Sent: $joinMessage');

      _channel!.stream.listen(
        (message) {
          printData("Received:", '$message');
          if (isSingle) {
            _handleSingleMessage(message);
          } else {
            _handleMessage(message);
          }
        },
        onError: (error) {
          printData("WebSocket Error:", '$error');
          _handleWebSocketError(isSingle: isSingle);
        },
        onDone: () {
          printData("WebSocket Closed", 'WebSocket Closed');
          _handleWebSocketError(isSingle: isSingle);
        },
      );
    } catch (e) {
      printData("Failed to connect:", ' $e');
      _handleWebSocketError(isSingle: isSingle);
    }
  }

  void _handleMessage(String message) {
    try {
      final Map<String, dynamic> outerJson = json.decode(message);
      if (outerJson.containsKey('data')) {
        final Map<String, dynamic> innerJson = json.decode(outerJson['data']);
        //  var jsonOrder = json.encode(innerJson);
        //  printData("Parsed Order 3:", ' $jsonOrder');
        printData("Parsed Order 2:", ' $innerJson');
        final updatedOrder = PackageOrderResponseModel.fromJson(innerJson);
        printData("Updated Order 2:", ' ${updatedOrder.id}');
        // Update or insert order into the list
        final index = allPackageOrders.indexWhere(
          (x) => x.id == updatedOrder.id,
        );
        if (index != -1) {
          allPackageOrders[index] = updatedOrder;
          packageOrderModel = updatedOrder;
        } else {
          allPackageOrders.insert(0, updatedOrder);
        }
        state = PackageOrderSuccess(
          message: 'Live Order Update',
          listData: allPackageOrders,
          data: packageOrderModel,
          visitCount: _visitCounter,
        );
      } else {
        printData("Json Error", 'Unexpected JSON Format');
      }
    } catch (e) {
      printData("Json Error", 'JSON Parsing Error: $e');
    }
  }

  void _handleSingleMessage(String message) {
    try {
      final Map<String, dynamic> outerJson = json.decode(message);
      if (outerJson.containsKey('data')) {
        final Map<String, dynamic> innerJson = json.decode(outerJson['data']);
        var jsonOrder = json.encode(innerJson);
        printData("Parsed Order 3:", ' $jsonOrder');
        final updatedOrder = PackageOrderResponseModel.fromJson(innerJson);
        packageOrderModel = updatedOrder;
        if (!mounted) {
          return;
        }
        state = PackageOrderSuccess(
          message: 'Live Order Update',
          data: packageOrderModel,
          visitCount: _visitCounter,
        );
      } else {
        printData("Json Error", 'Unexpected JSON Format');
      }
    } catch (e) {
      printData("Json Error", 'JSON Parsing Error: $e');
    }
  }

  void _handleWebSocketError({required bool isSingle}) {
    if (!isConnected) return;

    isConnected = false;
    reconnectionAttempts++;
    int delay = 2 ^ reconnectionAttempts;
    delay = delay < 60 ? delay : 60;

    Future.delayed(Duration(seconds: delay), () {
      _connectWebSocket(isSingle: isSingle);
    });
  }

  Future<void> disconnect() async {
    await _channel?.sink.close();
    printData("Disconnected", '❌ Disconnected from WebSocket');
  }

  //State Orders

  Future<void> getPackageOrderData({
    required String id,
    bool forceRefresh = false,
  }) async {
    try {
      if (!_fetchedOrdersById.containsKey(id) || forceRefresh) {
        if (!_fetchedOrdersById.containsKey(id)) {
          state = const PackageOrderLoading();
        } else {
          state = const PackageOrderRefreshing();
        }
        PackageOrderResponseModel response2 = await packageOrder
            .getPackageOrdersData(orderId: id);
        packageOrderModel = response2;
        _fetchedOrdersById[id] = true;
        state = PackageOrderSuccess<PackageOrderResponseModel>(
          message: "${response2.id} Notification Loaded",
          data: response2,
        );
      }
    } catch (e) {
      state = PackageOrderFailure(error: e.toString());
    }
  }

  Future<void> getAllPackageOrderData({bool forceRefresh = false}) async {
    _visitCounter++;
    printData("PackageOrderState Visit Count:", "$_visitCounter");
    try {
      if (_visitCounter == 1) {
        if (!_hasFetchedOrders || forceRefresh) {
          if (!_hasFetchedOrders) {
            state = const PackageOrderLoading();
          } else {
            state = const PackageOrderRefreshing();
          }

          final response = await packageOrder.getAllPackageOrdersData();
          allPackageOrders = response;
          _hasFetchedOrders = true;
          state = PackageOrderSuccess<PackageOrderResponseModel>(
            message: "All Accommodations Loaded",
            listData: response,
            visitCount: _visitCounter,
          );
        }
      } else {
        if (!mounted) {
          state = PackageOrderLoading();
          return;
        }
        final response = await packageOrder.getAllPackageOrdersData();
        allPackageOrders = response;
        _hasFetchedOrders = true;
        state = PackageOrderSuccess<PackageOrderResponseModel>(
          message: "All Accommodations Loaded",
          listData: allPackageOrders,
          visitCount: _visitCounter,
        );
      }
    } catch (e) {
      state = PackageOrderFailure(error: e.toString());
    }
  }
}

final streamRepositoryProvider = Provider<PackageOrderService>(
  (ref) => PackageOrderService(),
);

final packageOrderProvider =
    StateNotifierProvider.autoDispose<PackageOrderNotifier, PackageOrderState>((
      ref,
    ) {
      final bookingService = ref.read(streamRepositoryProvider);
      return PackageOrderNotifier(packageOrder: bookingService);
    });

// New One Stream
class NewPackageOrderStateService {
  late Stream<List<PackageOrderResponseModel>> _stream;
  packageOrderInit() async {
    var packageOrderService = PackageOrderService();
    _stream = packageOrderService.getAllStreamPackageOrdersData();
    final result = _stream;
    return result;
  }

  Stream<List<PackageOrderResponseModel>> get stream {
    return _stream;
  }
}

final streamRepo = Provider<NewPackageOrderStateService>(
  (ref) => NewPackageOrderStateService(),
);
final getListPackageOrder =
    StreamProvider.autoDispose<List<PackageOrderResponseModel>>((ref) {
      final streamServices = ref.watch(streamRepo);
      ref.refreshIn(const Duration(seconds: 5));
      streamServices.packageOrderInit();
      return streamServices.stream;
    });
