import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/features/account/states/account_provider.dart';
import 'package:ginilog_customer_app/core/features/account/view/report/statement_report_page.dart';
import 'package:ginilog_customer_app/core/features/bookings/state/customer_booked_state.dart';
import 'package:ginilog_customer_app/main.dart';

class StatementReportScreen extends ConsumerStatefulWidget {
  const StatementReportScreen({super.key});

  @override
  ConsumerState<StatementReportScreen> createState() =>
      StatementReportScreenController();
}

class StatementReportScreenController
    extends ConsumerState<StatementReportScreen>
    with RouteAware {
  late CustomerBookedStateNotifier bookingsProvider;
  @override
  void initState() {
    super.initState();
    // User Data
    final accountProviderd = ref.read(accountProvider.notifier);
    accountProviderd.getAccount();
    accountProviderd.userData;
    bookingsProvider = ref.read(customerBookedStateProvider.notifier);
    // First-time load
    Future.microtask(() {
      bookingsProvider.getAllCustomerReservationData();
    });
    // bookingsProvider.getAllCustomerReservationData(refresh: true);
  }

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    // Subscribe to route changes
    routeObserver.subscribe(this, ModalRoute.of(context)!);
  }

  @override
  void dispose() {
    // Unsubscribe
    routeObserver.unsubscribe(this);
    super.dispose();
  }

  @override
  void didPopNext() {
    // Called when user navigates back to this screen
    bookingsProvider.getAllCustomerReservationData(refresh: true);
  }

  @override
  Widget build(BuildContext context) {
    return StatementReportScreenView(this);
  }
}
