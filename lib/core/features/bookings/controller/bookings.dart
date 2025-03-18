import 'package:ginilog_customer_app/core/components/helpers/globals.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/features/account/states/account_provider.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/accomodation_response_model.dart';
import 'package:ginilog_customer_app/core/features/bookings/state/booking_state.dart';
import 'package:ginilog_customer_app/core/features/bookings/view/bookings.dart';

class BookingsScreen extends ConsumerStatefulWidget {
  const BookingsScreen({super.key});

  @override
  ConsumerState<BookingsScreen> createState() => BookingsScreenController();
}

class BookingsScreenController extends ConsumerState<BookingsScreen>
    with TickerProviderStateMixin {
  String? profilePicture = "";
  List<AccomodationResponseModel> allAccomodations = [];
  @override
  void initState() {
    super.initState();
    globals.updateHomeLoaded();
    // User Data
    final accountProviderd = ref.read(accountProvider.notifier);
    accountProviderd.getAccount();
    accountProviderd.userData;
    profilePicture = accountProviderd.userData?.profilePicture ??
        "${globals.profilePicture}";
    // Logistics Company
    final bookingsProvider = ref.read(bookingProvider.notifier);
    bookingsProvider.getAllAccomodationData();
    allAccomodations = bookingsProvider.allAccomodations;
  }

  @override
  Widget build(BuildContext context) {
    return BookingsScreenView(this);
  }
}
