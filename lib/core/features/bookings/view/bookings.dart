import 'package:ginilog_customer_app/core/components/architecture/mvc.dart';
import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';
import 'package:ginilog_customer_app/core/features/bookings/controller/bookings.dart';
import 'package:ginilog_customer_app/core/features/bookings/state/booking_state.dart';
import 'package:ginilog_customer_app/core/features/bookings/view/all_booking_list.dart';
import 'package:shimmer/shimmer.dart';

class BookingsScreenView
    extends StatelessView<BookingsScreen, BookingsScreenController> {
  const BookingsScreenView(super.state, {super.key});

  @override
  Widget build(BuildContext context) {
    final notifier = controller.ref.watch(bookingProvider.notifier);
    final state = controller.ref.watch(bookingProvider);
    final isLoading = state is BookingLoading && !state.hasLoadedInitially;
    final bookings = notifier.allAccomodations;
    return Scaffold(
      backgroundColor: AppColors.white,
      appBar: buildFlexibleAppBar(
        context: context,
        showBackButton: false,
        title: AppText(
          isBody: true,
          text: "Bookings",
          textAlign: TextAlign.start,
          fontSize: 18,
          color: AppColors.black,
          fontStyle: FontStyle.normal,
          fontWeight: FontWeight.w800,
        ),
      ),
      body: SafeArea(
        child: RefreshIndicator(
          onRefresh: () async {
            await notifier.getAllAccomodationData(forceRefresh: true);
          },
          child: Builder(
            builder: (context) {
              // if (state is BookingFailure) {
              //   return Center(child: Text("Error: ${state.error}"));
              // }

              if (isLoading) {
                return ListView.builder(
                  itemCount: 4,
                  padding: const EdgeInsets.symmetric(horizontal: 16),
                  itemBuilder: (context, index) {
                    return buildBookingShimmerCard(context);
                  },
                );
              }

              return BookingsListTab(allAccomodations: bookings);
            },
          ),
        ),
      ),
    );
  }
}

Widget buildBookingShimmerCard(BuildContext context) {
  return Shimmer.fromColors(
    baseColor: Colors.grey.shade300,
    highlightColor: Colors.grey.shade100,
    child: Container(
      width: SizeConfig.widthAdjusted(100),
      margin: const EdgeInsets.symmetric(vertical: 10),
      decoration: BoxDecoration(
        color: AppColors.white,
        borderRadius: const BorderRadius.all(Radius.circular(10)),
      ),
      child: Padding(
        padding: const EdgeInsets.all(8.0),
        child: Column(
          children: [
            Container(
              width: SizeConfig.widthAdjusted(100),
              height: 200,
              decoration: BoxDecoration(
                color: Colors.white,
                borderRadius: const BorderRadius.only(
                  topLeft: Radius.circular(10),
                  topRight: Radius.circular(10),
                ),
              ),
            ),
            const SizedBox(height: 10),
            Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    Container(height: 20, width: 150, color: Colors.white),
                    const Spacer(),
                    Container(height: 20, width: 80, color: Colors.white),
                  ],
                ),
                const SizedBox(height: 10),
                Container(
                  height: 15,
                  width: SizeConfig.widthAdjusted(100) * 0.8,
                  color: Colors.white,
                ),
                const SizedBox(height: 5),
                Container(
                  height: 15,
                  width: SizeConfig.widthAdjusted(100) * 0.6,
                  color: Colors.white,
                ),
              ],
            ),
          ],
        ),
      ),
    ),
  );
}
