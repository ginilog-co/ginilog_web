// ignore_for_file: deprecated_member_use

import 'package:ginilog_customer_app/core/components/utils/app_buttons.dart';
import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/money_formatter.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/accomodation_reservations_response_model.dart';
import 'package:ginilog_customer_app/core/features/bookings/view/accommodation/view_detail_accomodation_reservation.dart';

class AccomodationReservationWidget extends ConsumerStatefulWidget {
  final AccomodationReservationResponseModel accomodationReservation;
  const AccomodationReservationWidget({
    super.key,
    required this.accomodationReservation,
  });

  @override
  ConsumerState<AccomodationReservationWidget> createState() =>
      _AccomodationReservationWidgetState();
}

class _AccomodationReservationWidgetState
    extends ConsumerState<AccomodationReservationWidget> {
  @override
  void initState() {
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    final data3 = widget.accomodationReservation;
    return Padding(
      padding: const EdgeInsets.only(left: 5.0, right: 5.0, top: 5.0),
      child: GestureDetector(
        onTap: () {
          navigateToRoute(
            context,
            ViewAccomodationReservationPage(reservation: data3),
          );
        },
        child: Card(
          elevation: 4,
          child: Container(
            width: SizeConfig.widthAdjusted(100),
            decoration: BoxDecoration(
              color: AppColors.white,
              borderRadius: const BorderRadius.all(Radius.circular(10)),
            ),
            child: Column(
              spacing: 5,
              crossAxisAlignment: CrossAxisAlignment.start,
              mainAxisAlignment: MainAxisAlignment.start,
              children: [
                Padding(
                  padding: const EdgeInsets.only(left: 8.0, right: 8, top: 4),
                  child: Row(
                    spacing: 5,
                    children: [
                      SvgPicture.asset('assets/svgs/time_clock.svg', width: 20),
                      AppText(
                        isBody: true,
                        text: "Check In: ${data3.checkInTime}",
                        textAlign: TextAlign.start,
                        fontSize: 55,
                        color: AppColors.black,
                        fontStyle: FontStyle.normal,
                        fontWeight: FontWeight.w500,
                      ),
                      Spacer(),
                      AppText(
                        isBody: true,
                        text: "Available",
                        textAlign: TextAlign.start,
                        fontSize: 15,
                        color: AppColors.black,
                        fontStyle: FontStyle.normal,
                        fontWeight: FontWeight.w500,
                      ),
                    ],
                  ),
                ),
                Divider(),
                Padding(
                  padding: const EdgeInsets.only(left: 8.0, right: 8),
                  child: Row(
                    spacing: 10,
                    mainAxisAlignment: MainAxisAlignment.start,
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      BoxSizer(
                        widthPercent: 20,
                        heightPercent: 10,
                        safeArea: true,
                        child: Container(
                          decoration: BoxDecoration(
                            borderRadius: BorderRadius.all(Radius.circular(5)),
                            image: DecorationImage(
                              fit: BoxFit.fill,
                              image: NetworkImage(data3.accomodationImage!),
                            ),
                          ),
                        ),
                      ),
                      Expanded(
                        child: Column(
                          spacing: 5,
                          mainAxisAlignment: MainAxisAlignment.start,
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            AppText(
                              isBody: true,
                              text: "${data3.accomodationName}",
                              textAlign: TextAlign.start,
                              fontSize: 15,
                              color: AppColors.black,
                              fontStyle: FontStyle.normal,
                              fontWeight: FontWeight.bold,
                            ),
                            Row(
                              children: [
                                AppText(
                                  isBody: false,
                                  text: "Room: ${data3.roomNumber}",
                                  textAlign: TextAlign.start,
                                  fontSize: 15,
                                  color: AppColors.black,
                                  fontStyle: FontStyle.normal,
                                  fontWeight: FontWeight.bold,
                                ),
                                Spacer(),
                                AppText(
                                  isBody: false,
                                  text: "${data3.roomType}",
                                  textAlign: TextAlign.start,
                                  fontSize: 15,
                                  color: AppColors.green,
                                  fontStyle: FontStyle.normal,
                                  fontWeight: FontWeight.bold,
                                ),
                              ],
                            ),
                            Row(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                SvgPicture.asset(
                                  'assets/svgs/loaction_icon.svg',
                                  width: 20,
                                ),
                                Expanded(
                                  child: AppText(
                                    isBody: false,
                                    text: "${data3.location}",
                                    textAlign: TextAlign.start,
                                    fontSize: 15,
                                    color: AppColors.green,
                                    fontStyle: FontStyle.normal,
                                    fontWeight: FontWeight.w400,
                                  ),
                                ),
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
                        text: moneyFormat(context, data3.roomPrice!.toDouble()),
                        textAlign: TextAlign.start,
                        fontSize: 15,
                        color: AppColors.black,
                        fontStyle: FontStyle.normal,
                        fontWeight: FontWeight.bold,
                      ),
                      Spacer(),
                      AppButton(
                        text: "View Now",
                        onPressed: () {
                          navigateToRoute(
                            context,
                            ViewAccomodationReservationPage(reservation: data3),
                          );
                        },
                        widthPercent: 25,
                        heightPercent: 4,
                        fontSize: 12,
                        btnColor: AppColors.primary,
                        isLoading: false,
                      ),
                    ],
                  ),
                ),
                addVerticalSpacing(2),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
