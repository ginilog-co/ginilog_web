import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/features/bookings/state/reservation_state.dart';
import 'package:ginilog_customer_app/core/features/bookings/view/accommodation/main_reservation_page.dart';
import 'package:ginilog_customer_app/main.dart';

class MainReservationScreen extends ConsumerStatefulWidget {
  const MainReservationScreen({super.key});

  @override
  ConsumerState<MainReservationScreen> createState() =>
      MainReservationScreenController();
}

class MainReservationScreenController
    extends ConsumerState<MainReservationScreen>
    with TickerProviderStateMixin, WidgetsBindingObserver, RouteAware {
  late ReservationStateNotifier bookingsProvider;
  String? profilePicture = "";
  @override
  void initState() {
    super.initState();
    bookingsProvider = ref.read(reservationStateProvider.notifier);
    // bookingsProvider.getAllAccomodationReservationData();
    // First-time load
    Future.microtask(() {
      bookingsProvider.getAllAccomodationReservationData();
    });
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
    bookingsProvider.getAllAccomodationReservationData(refresh: true);
  }

  @override
  Widget build(BuildContext context) {
    return MainReservationScreenView(this);
  }
}
