import 'package:ginilog_customer_app/core/components/helpers/globals.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/features/account/states/account_provider.dart';
import 'package:ginilog_customer_app/core/features/bookings/state/booking_state.dart';
import 'package:ginilog_customer_app/core/features/bookings/view/bookings.dart';

class BookingsScreen extends ConsumerStatefulWidget {
  const BookingsScreen({super.key});

  @override
  ConsumerState<BookingsScreen> createState() => BookingsScreenController();
}

class BookingsScreenController extends ConsumerState<BookingsScreen>
    with TickerProviderStateMixin, WidgetsBindingObserver {
  late BookingNotifier bookingsProvider;
  String? profilePicture = "";

  @override
  void initState() {
    super.initState();

    // User Data
    final accountProviderd = ref.read(accountProvider.notifier);
    accountProviderd.getAccount();
    accountProviderd.userData;
    profilePicture =
        accountProviderd.userData?.profilePicture ??
        "${globals.profilePicture}";
    bookingsProvider = ref.read(bookingProvider.notifier);
    // First-time load
    Future.microtask(() {
      bookingsProvider.getAllAccomodationData();
    });
  }

  @override
  Widget build(BuildContext context) {
    return BookingsScreenView(this);
  }
}
