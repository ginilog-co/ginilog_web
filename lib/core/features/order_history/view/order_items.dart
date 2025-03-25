// ignore_for_file: deprecated_member_use

import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/features/order_history/model/package_orders_model.dart';
import 'package:ginilog_customer_app/core/features/order_history/view/order_details.dart';
import 'package:ginilog_customer_app/core/features/order_history/view/package_information.dart';

class ActiveOrderItem extends ConsumerStatefulWidget {
  final PackageOrderResponseModel order;
  final String userPhone;
  const ActiveOrderItem(
      {super.key, required this.order, required this.userPhone});

  @override
  ConsumerState<ActiveOrderItem> createState() => _ActiveOrderItemState();
}

class _ActiveOrderItemState extends ConsumerState<ActiveOrderItem> {
  @override
  Widget build(BuildContext context) {
    final data3 = widget.order;
    // DateTime dt2 = DateTime.parse(data3.createdAt!.toLocal().toString());
    // String date = DateFormat("E, MMM d hh:mm a").format(dt2.toLocal());
    TextScaler textScaler = MediaQuery.of(context).textScaler;

    return Padding(
      padding: const EdgeInsets.only(left: 5.0, right: 5.0, top: 5.0),
      child: GestureDetector(
        onTap: () {
          if (data3.paymentStatus == true) {
            navigateToRoute(
                context,
                OrderDetailsPage(
                  order: data3,
                ));
          } else {
            navigateToRoute(
                context,
                PackageInformationPage(
                  order: data3,
                  userPhone: widget.userPhone.toString(),
                ));
          }
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
                          Image.asset(
                            'assets/images/order_icon.png',
                            width: 20,
                            height: 20,
                          ),
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
                                        text: " ${data3.itemName} ",
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
                                    text: "#${data3.trackingNum}",
                                    textAlign: TextAlign.start,
                                    fontSize: 35,
                                    color: AppColors.black,
                                    fontStyle: FontStyle.normal,
                                    fontWeight: FontWeight.bold),
                                RichText(
                                  textAlign: TextAlign.start,
                                  textScaler: textScaler,
                                  text: TextSpan(
                                    style: const TextStyle(color: Colors.black),
                                    children: [
                                      TextSpan(
                                        text: "Should Arrive in",
                                        style: TextStyle(
                                          color: Colors.black,
                                          fontWeight: FontWeight.w400,
                                          fontSize: fontSized(context, 35),
                                          fontFamily: "Inter",
                                        ),
                                      ),
                                      TextSpan(
                                        text: " ${data3.expectedDeliveryTime} ",
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
                                  text: data3.orderStatus ==
                                              OrderClassState.open ||
                                          data3.orderStatus ==
                                              OrderClassState.booked ||
                                          data3.orderStatus ==
                                              OrderClassState.picked ||
                                          data3.orderStatus ==
                                              OrderClassState.inTransit
                                      ? "Track"
                                      : "View",
                                  textAlign: TextAlign.center,
                                  fontSize: 32,
                                  color: AppColors.white,
                                  fontStyle: FontStyle.normal,
                                  fontWeight: FontWeight.w900),
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
        ),
      ),
    );
  }
}
