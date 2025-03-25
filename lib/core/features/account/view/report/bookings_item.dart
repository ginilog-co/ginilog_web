import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/features/account/view/report/view_bookings_receipt.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/customer_book_response_model.dart';

class CustomerItemPage extends ConsumerStatefulWidget {
  final CustomerBookResponseModel accomodation;
  const CustomerItemPage({super.key, required this.accomodation});

  @override
  ConsumerState<CustomerItemPage> createState() => _CustomerItemPageState();
}

class _CustomerItemPageState extends ConsumerState<CustomerItemPage> {
  @override
  Widget build(BuildContext context) {
    TextScaler textScaler = MediaQuery.of(context).textScaler;

    final data3 = widget.accomodation;
    // DateTime dt2 = DateTime.parse(data3.createdAt!.toLocal().toString());
    // String date = DateFormat("E, MMM d hh:mm a").format(dt2.toLocal());

    return Padding(
      padding: const EdgeInsets.only(left: 5.0, right: 5.0, top: 5.0),
      child: GestureDetector(
        onTap: () {
          navigateToRoute(
              context,
              ViewCustomerBookReceiptPage(
                order: data3,
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
              crossAxisAlignment: CrossAxisAlignment.start,
              mainAxisAlignment: MainAxisAlignment.start,
              children: [
                Padding(
                  padding: const EdgeInsets.all(8.0),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    mainAxisAlignment: MainAxisAlignment.start,
                    children: [
                      Row(
                        spacing: 8,
                        crossAxisAlignment: CrossAxisAlignment.center,
                        mainAxisAlignment: MainAxisAlignment.start,
                        children: [
                          Expanded(
                            child: Column(
                              spacing: 5,
                              crossAxisAlignment: CrossAxisAlignment.start,
                              mainAxisAlignment: MainAxisAlignment.start,
                              children: [
                                RichText(
                                  textAlign: TextAlign.start,
                                  textScaler: textScaler,
                                  text: TextSpan(
                                    style: const TextStyle(color: Colors.black),
                                    children: [
                                      TextSpan(
                                        text: "",
                                        style: TextStyle(
                                          color: Colors.black,
                                          fontWeight: FontWeight.w400,
                                          fontSize: fontSized(context, 35),
                                          fontFamily: "Inter",
                                        ),
                                      ),
                                      TextSpan(
                                        text: " ${data3.ticketNum} ",
                                        style: TextStyle(
                                          color: Colors.black,
                                          fontWeight: FontWeight.bold,
                                          fontSize: fontSized(context, 35),
                                          fontFamily: "Mulish",
                                        ),
                                      ),
                                    ],
                                  ),
                                ),
                                AppText(
                                    isBody: true,
                                    text: "Room: ${data3.roomNumber}",
                                    textAlign: TextAlign.start,
                                    fontSize: 35,
                                    color: AppColors.black,
                                    fontStyle: FontStyle.normal,
                                    fontWeight: FontWeight.bold),
                                RichText(
                                  textAlign: TextAlign.start,
                                  text: TextSpan(
                                    style: const TextStyle(color: Colors.black),
                                    children: [
                                      TextSpan(
                                        text: "Apartment: ",
                                        style: TextStyle(
                                          color: Colors.black,
                                          fontWeight: FontWeight.w400,
                                          fontSize: fontSized(context, 35),
                                          fontFamily: "Inter",
                                        ),
                                      ),
                                      TextSpan(
                                        text: " ${data3.accomodationType} ",
                                        style: TextStyle(
                                          color: Colors.black,
                                          fontWeight: FontWeight.bold,
                                          fontSize: fontSized(context, 35),
                                          fontFamily: "Mulish",
                                        ),
                                      ),
                                    ],
                                  ),
                                ),
                              ],
                            ),
                          ),
                          Container(
                            margin: const EdgeInsets.only(
                                left: 40, right: 5, top: 10, bottom: 10),
                            decoration: BoxDecoration(
                              color: AppColors.primary,
                              borderRadius: BorderRadius.circular(15),
                              boxShadow: const [
                                BoxShadow(
                                    blurRadius: 15.0,
                                    color: Color.fromRGBO(0, 0, 0, 0.2)),
                              ],
                            ),
                            child: Padding(
                              padding: EdgeInsets.only(
                                  left: 10, right: 10, top: 10, bottom: 10),
                              child: AppText(
                                  isBody: true,
                                  text: "View",
                                  textAlign: TextAlign.center,
                                  fontSize: 32,
                                  color: AppColors.white,
                                  fontStyle: FontStyle.normal,
                                  fontWeight: FontWeight.w900),
                            ),
                          ),
                        ],
                      ),
                      addVerticalSpacing(context, 5),
                      RichText(
                        textAlign: TextAlign.start,
                        text: TextSpan(
                          style: const TextStyle(color: Colors.black),
                          children: [
                            TextSpan(
                              text: "From: ",
                              style: TextStyle(
                                color: Colors.black,
                                fontWeight: FontWeight.w400,
                                fontSize: fontSized(context, 15),
                                fontFamily: "Inter",
                              ),
                            ),
                            TextSpan(
                              text: " ${data3.reservationStartDate} ",
                              style: TextStyle(
                                color: Colors.black,
                                fontWeight: FontWeight.bold,
                                fontSize: fontSized(context, 25),
                                fontFamily: "Mulish",
                              ),
                            ),
                            TextSpan(
                              text: "to: ",
                              style: TextStyle(
                                color: Colors.black,
                                fontWeight: FontWeight.w400,
                                fontSize: fontSized(context, 15),
                                fontFamily: "Inter",
                              ),
                            ),
                            TextSpan(
                              text: " ${data3.reservationEndDate} ",
                              style: TextStyle(
                                color: Colors.black,
                                fontWeight: FontWeight.bold,
                                fontSize: fontSized(context, 25),
                                fontFamily: "Mulish",
                              ),
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
