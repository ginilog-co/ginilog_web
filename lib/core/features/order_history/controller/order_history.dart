import 'package:ginilog_customer_app/core/components/helpers/globals.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/features/account/states/account_provider.dart';
import 'package:ginilog_customer_app/core/features/home/states/home_state.dart';
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
  late List<PackageOrderResponseModel> allOrders;
  String? profilePicture = "";
  String userPhone = "";
  @override
  void initState() {
    globals.updateHomeLoaded();
    tabController = TabController(length: 4, vsync: this);
    super.initState();
    // User Data
    final accountProviderd = ref.read(accountProvider.notifier);
    accountProviderd.getAccount();
    accountProviderd.userData;
    profilePicture = accountProviderd.userData?.profilePicture ??
        "${globals.profilePicture}";
    userPhone = accountProviderd.userData?.phoneNo ?? "";
    // Logistics Company
    final station = ref.read(homeProvider.notifier);
    station.getAllLogisticsData();
    station.getAllRiderData();
    // Notification
    station.getAllNotificationData();

    // Order
    final orderProvider = ref.read(packageOrderProvider.notifier);
    orderProvider.getAllPackageOrderData();
    allOrders = orderProvider.allPackageOrders;
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
