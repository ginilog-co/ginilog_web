import 'package:ginilog_customer_app/core/components/helpers/globals.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/features/account/states/account_provider.dart';
import 'package:ginilog_customer_app/core/features/order_history/model/package_orders_model.dart';
import 'package:ginilog_customer_app/core/features/order_history/states/order_state.dart';
import 'package:ginilog_customer_app/core/features/order_history/view/order_history.dart';

class OrderHistoryScreen extends ConsumerStatefulWidget {
  const OrderHistoryScreen({super.key});

  @override
  ConsumerState<OrderHistoryScreen> createState() =>
      OrderHistoryScreenController();
}

class OrderHistoryScreenController extends ConsumerState<OrderHistoryScreen>
    with TickerProviderStateMixin {
  late TabController tabController;
  late List<PackageOrderResponseModel> allOrders = [];
  String? profilePicture = "";
  String userPhone = "";
  late PackageOrderNotifier orderProvider;
  @override
  void initState() {
    tabController = TabController(length: 4, vsync: this);
    super.initState();
    // User Data
    final accountProviderd = ref.read(accountProvider.notifier);
    accountProviderd.getAccount();
    accountProviderd.userData;
    profilePicture =
        accountProviderd.userData?.profilePicture ??
        "${globals.profilePicture}";
    userPhone = accountProviderd.userData?.phoneNo ?? "";

    // Order
    orderProvider = ref.read(packageOrderProvider.notifier);
    Future.microtask(() {
      orderProvider.getAllPackageOrderData();
      allOrders = orderProvider.allPackageOrders;
    });
    // Connect WebSocket and join this order
    orderProvider.connectAndJoinOrder(
      orderId: globals.userId!,
      isSingle: false,
    );
  }

  @override
  void dispose() {
    orderProvider.disconnect();
    super.dispose();
  }

  selectTabb(int index) {
    tabController.animateTo(index);
    setState(() {});
  }

  @override
  Widget build(BuildContext context) {
    return OrderHistoryScreenView(this);
  }
}
