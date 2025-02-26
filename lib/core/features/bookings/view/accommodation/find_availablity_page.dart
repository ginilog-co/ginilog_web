import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/accomodation_reservations_response_model.dart';
import 'package:ginilog_customer_app/core/features/bookings/state/booking_state.dart';
import 'package:ginilog_customer_app/core/features/bookings/widget/find_reservation_widget.dart';

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
  List<AccomodationReservationResponseModel> filteredTickets = [];

  @override
  void initState() {
    super.initState();

    final bookingsProvider = ref.read(bookingProvider.notifier);
    bookingsProvider.getAllAccomodationReservationData();
    filteredTickets =
        bookingsProvider.allAccomodationReservations.where((test) {
      return test.accomodationId == widget.accommodationId &&
          test.isBooked == false;
    }).toList();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        backgroundColor: AppColors.white,
        appBar: PreferredSize(
            preferredSize: Size.fromHeight(SizeConfig.heightAdjusted(12)),
            child: Padding(
              padding: EdgeInsets.only(top: SizeConfig.heightAdjusted(10)),
              child: Column(
                children: [
                  GlobalBackButton(
                      backText: '${widget.accommodationName} Available Rooms',
                      showBackButton: true),
                ],
              ),
            )),
        body: SingleChildScrollView(
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
                          addVerticalSpacing(context, 55),
                          const AppText(
                              isBody: false,
                              text: "Nothing to show here",
                              textAlign: TextAlign.start,
                              fontSize: 78,
                              color: AppColors.black,
                              fontStyle: FontStyle.normal,
                              fontWeight: FontWeight.bold),
                          const AppText(
                              isBody: true,
                              text:
                                  "We don't have any Accomodation Reservations at the moment",
                              textAlign: TextAlign.center,
                              fontSize: 70,
                              color: AppColors.black,
                              fontStyle: FontStyle.normal,
                              fontWeight: FontWeight.normal),
                        ],
                      ),
                    )
                  : ListView.builder(
                      itemCount: filteredTickets.length,
                      shrinkWrap: true,
                      physics: const NeverScrollableScrollPhysics(),
                      itemBuilder: (context, index) => FindReservationWidget(
                            accomodationReservation: filteredTickets[index],
                          ))
            ],
          ),
        ));
  }
}
