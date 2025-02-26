import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/money_formatter.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/flight_ticket_response_model.dart';

class FlightTicketWidget extends ConsumerStatefulWidget {
  final FlightTicketResponseModel flightTicket;
  const FlightTicketWidget({super.key, required this.flightTicket});

  @override
  ConsumerState<FlightTicketWidget> createState() => _FlightTicketWidgetState();
}

class _FlightTicketWidgetState extends ConsumerState<FlightTicketWidget> {
  @override
  Widget build(BuildContext context) {
    final data3 = widget.flightTicket;
    return Padding(
      padding: const EdgeInsets.only(left: 5.0, right: 5.0, top: 5.0),
      child: GestureDetector(
        onTap: () {
          // navigateToRoute(
          //     context,
          //     flightTicketDetailsPage(
          //       flightTicket: data3,
          //     ));
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
              spacing: 5,
              crossAxisAlignment: CrossAxisAlignment.start,
              mainAxisAlignment: MainAxisAlignment.start,
              children: [
                Padding(
                  padding: const EdgeInsets.only(left: 8.0, right: 8, top: 4),
                  child: Row(
                    children: [
                      Expanded(
                        child: AppText(
                            isBody: true,
                            text: "${data3.airlineName}",
                            textAlign: TextAlign.start,
                            fontSize: 85,
                            color: AppColors.black,
                            fontStyle: FontStyle.normal,
                            fontWeight: FontWeight.bold),
                      ),
                      AppText(
                          isBody: true,
                          text: data3.isReturn == true ? "Return" : "No Return",
                          textAlign: TextAlign.start,
                          fontSize: 75,
                          color: AppColors.blue,
                          fontStyle: FontStyle.normal,
                          fontWeight: FontWeight.bold),
                    ],
                  ),
                ),
                Divider(),
                Padding(
                  padding: const EdgeInsets.only(left: 8.0, right: 8),
                  child: Row(
                    spacing: 10,
                    children: [
                      AppText(
                          isBody: false,
                          text: "${data3.dapature}",
                          textAlign: TextAlign.start,
                          fontSize: 85,
                          color: AppColors.black,
                          fontStyle: FontStyle.normal,
                          fontWeight: FontWeight.bold),
                      Icon(
                        Icons.arrow_forward_outlined,
                        size: 15,
                      ),
                      AppText(
                          isBody: false,
                          text: "${data3.destination}",
                          textAlign: TextAlign.start,
                          fontSize: 85,
                          color: AppColors.black,
                          fontStyle: FontStyle.normal,
                          fontWeight: FontWeight.bold),
                      Spacer(),
                      AppText(
                          isBody: false,
                          text: "${data3.ticketNum}",
                          textAlign: TextAlign.start,
                          fontSize: 85,
                          color: AppColors.blue,
                          fontStyle: FontStyle.normal,
                          fontWeight: FontWeight.bold),
                    ],
                  ),
                ),
                Padding(
                  padding: const EdgeInsets.only(left: 8.0, right: 8),
                  child: Row(
                    spacing: 10,
                    children: [
                      SvgPicture.asset(
                        'assets/svgs/time_clock.svg',
                        width: 20,
                      ),
                      AppText(
                          isBody: false,
                          text: "${data3.availabeTimeInterval}",
                          textAlign: TextAlign.start,
                          fontSize: 85,
                          color: AppColors.black,
                          fontStyle: FontStyle.normal,
                          fontWeight: FontWeight.bold),
                      Spacer(),
                      AppText(
                          isBody: false,
                          text: "${data3.ticketType}",
                          textAlign: TextAlign.start,
                          fontSize: 85,
                          color: AppColors.black,
                          fontStyle: FontStyle.normal,
                          fontWeight: FontWeight.bold),
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
                          text: moneyFormat(
                              context, data3.ticketPrice!.toDouble()),
                          textAlign: TextAlign.start,
                          fontSize: 75,
                          color: AppColors.black,
                          fontStyle: FontStyle.normal,
                          fontWeight: FontWeight.bold),
                      Spacer(),
                      GestureDetector(
                        onTap: () async {},
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
