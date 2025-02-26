import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/money_formatter.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/features/bookings/controller/book_reservation.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/accomodation_reservations_response_model.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/accomodation_response_model.dart';
import 'package:ginilog_customer_app/core/features/bookings/state/booking_state.dart';

class FindReservationWidget extends ConsumerStatefulWidget {
  final AccomodationReservationResponseModel accomodationReservation;
  const FindReservationWidget(
      {super.key, required this.accomodationReservation});

  @override
  ConsumerState<FindReservationWidget> createState() =>
      _FindReservationWidgetState();
}

class _FindReservationWidgetState extends ConsumerState<FindReservationWidget> {
  AccomodationResponseModel accomodationData = AccomodationResponseModel();
  @override
  void initState() {
    super.initState();
    final bookings = ref.read(bookingProvider.notifier);
    bookings.getAllAccomodationData();
    accomodationData = bookings
        .fetchAccomodationById(widget.accomodationReservation.accomodationId!)!;
  }

  @override
  Widget build(BuildContext context) {
    final data3 = widget.accomodationReservation;

    // DateTime dt2 = DateTime.parse(data3.createdAt.toString());
    // String date = DateFormat("E, MMM d hh:mm a").format(dt2);
    // // DateTime dt3 = data3.updatedAt!;
    // // String time = DateFormat("hh:mm a").format(dt3);
    return Padding(
      padding: const EdgeInsets.only(left: 5.0, right: 5.0, top: 5.0),
      child: GestureDetector(
        onTap: () {
          navigateToRoute(
              context,
              BookReservationScreen(
                reservationId: widget.accomodationReservation.id.toString(),
                bookingPrice: widget.accomodationReservation.roomPrice!,
                maximumNoOfGuest:
                    widget.accomodationReservation.maximumNoOfGuest!.toInt(),
              ));
        },
        child: Card(
          elevation: 4,
          child: Container(
            width: getScreenWidth(context),
            decoration: BoxDecoration(
              color: AppColors.white,
              borderRadius: const BorderRadius.all(
                Radius.circular(10),
              ),
            ),
            child: Column(
              spacing: 1,
              crossAxisAlignment: CrossAxisAlignment.start,
              mainAxisAlignment: MainAxisAlignment.start,
              children: [
                Padding(
                  padding: const EdgeInsets.only(left: 8.0, right: 8, top: 4),
                  child: Row(
                    spacing: 5,
                    children: [
                      SvgPicture.asset(
                        'assets/svgs/time_clock.svg',
                        width: 20,
                      ),
                      AppText(
                          isBody: true,
                          text: "Check In: ${accomodationData.checkInTime}",
                          textAlign: TextAlign.start,
                          fontSize: 75,
                          color: AppColors.black,
                          fontStyle: FontStyle.normal,
                          fontWeight: FontWeight.w500),
                      Spacer(),
                      AppText(
                          isBody: true,
                          text: data3.isBooked == true
                              ? "Not Available"
                              : "Available",
                          textAlign: TextAlign.start,
                          fontSize: 75,
                          color: AppColors.black,
                          fontStyle: FontStyle.normal,
                          fontWeight: FontWeight.w500),
                    ],
                  ),
                ),
                Divider(),
                Padding(
                  padding: const EdgeInsets.only(left: 8.0, right: 8),
                  child: Row(
                    spacing: 5,
                    mainAxisAlignment: MainAxisAlignment.start,
                    crossAxisAlignment: CrossAxisAlignment.center,
                    children: [
                      Container(
                        height: 50,
                        width: 50,
                        decoration: BoxDecoration(
                            borderRadius: BorderRadius.all(Radius.circular(5)),
                            image: DecorationImage(
                                fit: BoxFit.fill,
                                image: NetworkImage(data3.accomodationImage!))),
                      ),
                      Expanded(
                        child: Column(
                          spacing: 5,
                          mainAxisAlignment: MainAxisAlignment.start,
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Row(
                              children: [
                                AppText(
                                    isBody: false,
                                    text: "Room: ${data3.roomNumber}",
                                    textAlign: TextAlign.start,
                                    fontSize: 85,
                                    color: AppColors.black,
                                    fontStyle: FontStyle.normal,
                                    fontWeight: FontWeight.bold),
                                Spacer(),
                                AppText(
                                    isBody: false,
                                    text: "${data3.roomType}",
                                    textAlign: TextAlign.start,
                                    fontSize: 85,
                                    color: AppColors.green,
                                    fontStyle: FontStyle.normal,
                                    fontWeight: FontWeight.bold),
                              ],
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),
                ),
                Divider(),
                Padding(
                  padding: const EdgeInsets.only(left: 8.0, right: 8),
                  child: Row(
                    children: [
                      AppText(
                          isBody: false,
                          text:
                              moneyFormat(context, data3.roomPrice!.toDouble()),
                          textAlign: TextAlign.start,
                          fontSize: 75,
                          color: AppColors.black,
                          fontStyle: FontStyle.normal,
                          fontWeight: FontWeight.bold),
                      Spacer(),
                      GestureDetector(
                        onTap: () async {
                          navigateToRoute(
                              context,
                              BookReservationScreen(
                                reservationId: widget.accomodationReservation.id
                                    .toString(),
                                bookingPrice:
                                    widget.accomodationReservation.roomPrice!,
                                maximumNoOfGuest: widget
                                    .accomodationReservation.maximumNoOfGuest!
                                    .toInt(),
                              ));
                        },
                        child: Container(
                          margin: const EdgeInsets.only(left: 20, right: 20),
                          decoration: BoxDecoration(
                            color: AppColors.primary,
                            borderRadius: BorderRadius.circular(5),
                            boxShadow: const [
                              BoxShadow(
                                  blurRadius: 15.0,
                                  color: Color.fromRGBO(0, 0, 0, 0.2)),
                            ],
                          ),
                          child: const Padding(
                            padding: EdgeInsets.only(
                                left: 25.0, right: 25, top: 10, bottom: 10),
                            child: AppText(
                                isBody: true,
                                text: "Book Now",
                                textAlign: TextAlign.center,
                                fontSize: 72,
                                color: AppColors.white,
                                fontStyle: FontStyle.normal,
                                fontWeight: FontWeight.w900),
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
                addVerticalSpacing(context, 2),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
