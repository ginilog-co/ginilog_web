import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/features/account/states/account_provider.dart';
import 'package:ginilog_customer_app/core/features/account/view/report/statement_report_page.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/customer_book_response_model.dart';
import 'package:ginilog_customer_app/core/features/bookings/state/booking_state.dart';
import 'package:ginilog_customer_app/core/features/order_history/model/package_orders_model.dart';
import 'package:ginilog_customer_app/core/features/order_history/states/order_state.dart';

class StatementReportScreen extends ConsumerStatefulWidget {
  const StatementReportScreen({super.key});

  @override
  ConsumerState<StatementReportScreen> createState() =>
      StatementReportScreenController();
}

class StatementReportScreenController
    extends ConsumerState<StatementReportScreen> with TickerProviderStateMixin {
  late TabController tabController;
  late List<PackageOrderResponseModel> allOrders = [];
  List<CustomerBookResponseModel> allAccomodations = [];

  @override
  void initState() {
    tabController = TabController(length: 2, vsync: this);
    super.initState();
    // User Data
    final accountProviderd = ref.read(accountProvider.notifier);
    accountProviderd.getAccount();
    accountProviderd.userData;

    // Order
    final orderProvider = ref.read(packageOrderProvider.notifier);
    final booking = ref.read(bookingProvider.notifier);
    orderProvider.getAllPackageOrderData();
    booking.getAllCustomerBookData();
    allOrders = orderProvider.allPackageOrders;
    allAccomodations = booking.allCustomerBooks;
  }

  selectTabb(int index) {
    tabController.animateTo(index);
    setState(() {});
  }

  @override
  Widget build(BuildContext context) {
    return StatementReportScreenView(this);
  }
}
