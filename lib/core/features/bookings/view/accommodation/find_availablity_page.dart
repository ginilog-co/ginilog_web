import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';
import 'package:ginilog_customer_app/core/features/bookings/state/reservation_state.dart';
import 'package:ginilog_customer_app/core/features/bookings/widget/find_reservation_widget.dart';
import 'package:shimmer/shimmer.dart';

import '../../../../components/utils/package_export.dart';

class FindReservationPage extends ConsumerStatefulWidget {
  const FindReservationPage({
    super.key,
    required this.accommodationId,
    required this.accommodationName,
  });
  final String accommodationId;
  final String accommodationName;
  @override
  ConsumerState<FindReservationPage> createState() =>
      _FindReservationPageState();
}

class _FindReservationPageState extends ConsumerState<FindReservationPage> {
  @override
  void initState() {
    super.initState();

    final bookingsProvider = ref.read(reservationStateProvider.notifier);

    // First-time load
    Future.microtask(() {
      bookingsProvider.getAllAccomodationReservationData();
    });
  }

  @override
  Widget build(BuildContext context) {
    final notifier = ref.watch(reservationStateProvider.notifier);
    final state = ref.watch(reservationStateProvider);
    final isLoading =
        state is ReservationStateLoading && !state.hasLoadedInitially;

    var filteredTickets =
        notifier.allAccomodationReservations.where((test) {
          return test.accomodationId == widget.accommodationId;
        }).toList();
    return Scaffold(
      backgroundColor: AppColors.white,
      appBar: buildFlexibleAppBar(
        context: context,

        title: AppText(
          isBody: true,
          text: "${widget.accommodationName} Available Rooms",
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
            await notifier.getAllAccomodationReservationData(refresh: true);
          },
          child: Builder(
            builder: (context) {
              if (isLoading) {
                return ListView.builder(
                  itemCount: 4,
                  padding: const EdgeInsets.symmetric(horizontal: 16),
                  itemBuilder: (context, index) {
                    return buildBookingShimmerCard(context);
                  },
                );
              }
              return SingleChildScrollView(
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.start,
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    filteredTickets.isEmpty
                        ? Center(
                          child: Column(
                            mainAxisAlignment: MainAxisAlignment.center,
                            crossAxisAlignment: CrossAxisAlignment.center,
                            children: [
                              addVerticalSpacing(5),
                              const AppText(
                                isBody: false,
                                text: "Nothing to show here",
                                textAlign: TextAlign.start,
                                fontSize: 18,
                                color: AppColors.black,
                                fontStyle: FontStyle.normal,
                                fontWeight: FontWeight.bold,
                              ),
                              const AppText(
                                isBody: true,
                                text:
                                    "We don't have any Accomodation Reservations at the moment",
                                textAlign: TextAlign.center,
                                fontSize: 15,
                                color: AppColors.black,
                                fontStyle: FontStyle.normal,
                                fontWeight: FontWeight.normal,
                              ),
                            ],
                          ),
                        )
                        : ListView.builder(
                          itemCount: filteredTickets.length,
                          shrinkWrap: true,
                          physics: const NeverScrollableScrollPhysics(),
                          itemBuilder:
                              (context, index) => FindReservationWidget(
                                accomodationReservation: filteredTickets[index],
                              ),
                        ),
                  ],
                ),
              );
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
