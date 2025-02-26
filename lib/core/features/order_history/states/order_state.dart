// ignore_for_file: use_build_context_synchronously

import 'package:ginilog_customer_app/core/components/extension/all_extension.dart';
import 'package:ginilog_customer_app/core/features/order_history/model/package_orders_model.dart';
import 'package:ginilog_customer_app/core/features/order_history/services/package_order_service.dart';

import '../../../components/utils/package_export.dart';

abstract class PackageOrderState {}

class PackageOrderInitial extends PackageOrderState {}

class PackageOrderLoading extends PackageOrderState {}

class PackageOrderSuccess extends PackageOrderState {
  final String message;
  PackageOrderSuccess({required this.message});
}

class PackageOrderUpdate extends PackageOrderState {
  final String message;
  PackageOrderUpdate({required this.message});
}

class PackageOrderFailure extends PackageOrderState {
  final String error;
  PackageOrderFailure({required this.error});
}

class PackageOrderNotifier extends StateNotifier<PackageOrderState> {
  final PackageOrderService packageOrder;
  List<PackageOrderResponseModel> allPackageOrders = [];

  PackageOrderResponseModel packageOrderModel = PackageOrderResponseModel();
  // Notifications

  PackageOrderNotifier({required this.packageOrder})
      : super(PackageOrderInitial());

  getPackageOrderData({required String orderId}) async {
    try {
      if (!mounted) {
        state = PackageOrderLoading();
        return;
      }
      PackageOrderResponseModel response2 =
          await packageOrder.getPackageOrdersData(orderId: orderId);
      packageOrderModel = response2;
      state =
          PackageOrderSuccess(message: "${response2.id} PackageOrder Loaded");
    } on Exception catch (e) {
      state = PackageOrderFailure(error: e.toString());
    }
  }

  getAllPackageOrderData() async {
    try {
      if (!mounted) {
        state = PackageOrderLoading();
        return;
      }
      List<PackageOrderResponseModel> response2 =
          await packageOrder.getAllPackageOrdersData();
      allPackageOrders = response2;
      state = PackageOrderSuccess(message: "All PackageOrders Loaded");
    } on Exception catch (e) {
      state = PackageOrderFailure(error: e.toString());
    }
  }
}

final streamRepositoryProvider =
    Provider<PackageOrderService>((ref) => PackageOrderService());
final packageOrderProvider =
    StateNotifierProvider<PackageOrderNotifier, PackageOrderState>((ref) {
  final PackageOrderService packageOrder = PackageOrderService();
  return PackageOrderNotifier(packageOrder: packageOrder);
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
    (ref) => NewPackageOrderStateService());
final getListPackageOrder =
    StreamProvider.autoDispose<List<PackageOrderResponseModel>>((ref) {
  final streamServices = ref.watch(streamRepo);
  ref.refreshIn(const Duration(seconds: 5));
  streamServices.packageOrderInit();
  return streamServices.stream;
});
