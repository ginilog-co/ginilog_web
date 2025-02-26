// ignore_for_file: use_build_context_synchronously, deprecated_member_use

import 'package:ginilog_customer_app/core/components/utils/app_buttons.dart';
import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/money_formatter.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';
import 'package:ginilog_customer_app/core/features/home/model/riders_response_model.dart';
import 'package:ginilog_customer_app/core/features/home/states/home_state.dart';
import 'package:ginilog_customer_app/core/features/order_history/model/package_orders_model.dart';
import 'package:ginilog_customer_app/core/features/order_history/view/map_tracking.dart';

class OrderDetailsPage extends ConsumerStatefulWidget {
  final PackageOrderResponseModel order;
  const OrderDetailsPage({super.key, required this.order});

  @override
  ConsumerState<OrderDetailsPage> createState() => _OrderDetailsPageState();
}

class _OrderDetailsPageState extends ConsumerState<OrderDetailsPage> {
  @override
  Widget build(BuildContext context) {
    final station = ref.read(homeProvider.notifier);
    station.getAllRiderData();

    RidersResponseModel? rider = RidersResponseModel();
    final data3 = widget.order;
    if (data3.riderId.toString().isNotEmpty) {
      rider = station.fetchRiderById(data3.riderId.toString());
    }
    DateTime dt2 = DateTime.parse(data3.createdAt.toString());
    // DateTime dt3 = data3.updatedAt!;
    String date = DateFormat("E, MMM d hh:mm a").format(dt2);
    // String time = DateFormat("hh:mm a").format(dt3);
    return Scaffold(
      backgroundColor: AppColors.white,
      appBar: PreferredSize(
          preferredSize: Size.fromHeight(SizeConfig.heightAdjusted(16)),
          child: Padding(
            padding: EdgeInsets.only(top: SizeConfig.heightAdjusted(10)),
            child: Column(
              children: [
                GlobalBackButton(
                  backText: 'Package Order Details',
                  showBackButton: true,
                  buttonElements: [],
                ),
              ],
            ),
          )),
      body: SingleChildScrollView(
        child: Padding(
          padding: const EdgeInsets.only(left: 15.0, right: 15.0, top: 0.0),
          child: Column(
            children: [
              Container(
                width: getScreenWidth(context),
                decoration: const BoxDecoration(
                    color: AppColors.grey5,
                    borderRadius: BorderRadius.only(
                        topLeft: Radius.circular(10),
                        topRight: Radius.circular(10)),
                    border: Border(
                      bottom:
                          BorderSide(color: AppColors.primaryDark, width: 2.5),
                    )),
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
                            children: [
                              const AppText(
                                  isBody: true,
                                  text: "Rider:",
                                  textAlign: TextAlign.start,
                                  fontSize: 75,
                                  color: AppColors.black,
                                  fontStyle: FontStyle.normal,
                                  maxLines: 1,
                                  fontWeight: FontWeight.bold),
                              addHorizontalSpacing(5),
                              Expanded(
                                child: data3.riderName!.isEmpty
                                    ? const AppText(
                                        isBody: true,
                                        text:
                                            "Gas Order Awaiting to Assign A Rider",
                                        textAlign: TextAlign.center,
                                        fontSize: 78,
                                        color: AppColors.primaryDark,
                                        fontStyle: FontStyle.normal,
                                        fontWeight: FontWeight.normal)
                                    : Row(
                                        children: [
                                          CircleAvatar(
                                            backgroundColor: AppColors.green,
                                            radius: 15,
                                            backgroundImage: NetworkImage(
                                              rider!.profilePicture == null
                                                  ? 'https://images.unsplash.com/'
                                                      'photo-1582106245687-cbb466a9f07f?ixlib='
                                                      'rb-1.2.1&ixid=MnwxMjA3fDB8MHxzZWFyY2'
                                                      'h8NXx8ZHJpbmtzfGVufDB8fDB8fA%3D%3D&auto'
                                                      '=format&fit=crop&w=800&q=60'
                                                  : rider.profilePicture!
                                                          .isEmpty
                                                      ? 'https://images.unsplash.com/'
                                                          'photo-1582106245687-cbb466a9f07f?ixlib='
                                                          'rb-1.2.1&ixid=MnwxMjA3fDB8MHxzZWFyY2'
                                                          'h8NXx8ZHJpbmtzfGVufDB8fDB8fA%3D%3D&auto'
                                                          '=format&fit=crop&w=800&q=60'
                                                      : rider.profilePicture!,
                                            ),
                                          ),
                                          addHorizontalSpacing(10),
                                          AppText(
                                              isBody: true,
                                              text:
                                                  "${rider.firstName} ${rider.lastName}",
                                              textAlign: TextAlign.start,
                                              fontSize: 75,
                                              color: AppColors.black,
                                              fontStyle: FontStyle.normal,
                                              maxLines: 1,
                                              fontWeight: FontWeight.bold),
                                          const Spacer(),
                                          data3.orderStatus ==
                                                      OrderClassState.open ||
                                                  data3.orderStatus ==
                                                      OrderClassState
                                                          .accepted ||
                                                  data3.orderStatus ==
                                                      OrderClassState.picked ||
                                                  data3.orderStatus ==
                                                      OrderClassState.ongoing ||
                                                  data3.orderStatus ==
                                                      OrderClassState.delivered
                                              ? GestureDetector(
                                                  onTap: () async {
                                                    Uri phoneNo = Uri.parse(
                                                        'tel:${rider!.phoneNo}');
                                                    if (await launchUrl(
                                                        phoneNo)) {
                                                      //dialer opened
                                                    } else {
                                                      //dailer is not opened
                                                    }
                                                  },
                                                  child: Container(
                                                    decoration: BoxDecoration(
                                                      color: AppColors.primary,
                                                      borderRadius:
                                                          BorderRadius.circular(
                                                              15),
                                                      boxShadow: const [
                                                        BoxShadow(
                                                            blurRadius: 15.0,
                                                            color:
                                                                Color.fromRGBO(
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    0.2)),
                                                      ],
                                                    ),
                                                    child: const Padding(
                                                      padding: EdgeInsets.only(
                                                          left: 25.0,
                                                          right: 25,
                                                          top: 15,
                                                          bottom: 15),
                                                      child: AppText(
                                                          isBody: true,
                                                          text: "Contact",
                                                          textAlign:
                                                              TextAlign.center,
                                                          fontSize: 26,
                                                          color:
                                                              AppColors.white,
                                                          fontStyle:
                                                              FontStyle.normal,
                                                          fontWeight: FontWeight
                                                              .normal),
                                                    ),
                                                  ),
                                                )
                                              : const SizedBox.shrink()
                                        ],
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
              Container(
                width: getScreenWidth(context),
                decoration: BoxDecoration(
                    color: AppColors.white,
                    borderRadius: const BorderRadius.only(
                        bottomLeft: Radius.circular(10),
                        bottomRight: Radius.circular(10)),
                    border: Border(
                      bottom: BorderSide(
                          color: AppColors.primaryDark.withOpacity(0.5),
                          width: 1.5),
                      right: BorderSide(
                          color: AppColors.primaryDark.withOpacity(0.5),
                          width: 1.5),
                      left: BorderSide(
                          color: AppColors.primaryDark.withOpacity(0.5),
                          width: 1.5),
                    )),
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
                            children: [
                              Image.asset('assets/images/rider_icon.png',
                                  width: 50, height: 50),
                              Spacer(),
                              AppText(
                                  isBody: true,
                                  text: "TN: ${data3.trackingNum}",
                                  textAlign: TextAlign.start,
                                  fontSize: 60,
                                  color: AppColors.warning,
                                  fontStyle: FontStyle.normal,
                                  fontWeight: FontWeight.bold),
                            ],
                          ),
                          Row(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            mainAxisAlignment: MainAxisAlignment.start,
                            children: [
                              const AppText(
                                  isBody: false,
                                  text: "Item Name:",
                                  textAlign: TextAlign.start,
                                  fontSize: 75,
                                  color: AppColors.black,
                                  fontStyle: FontStyle.normal,
                                  fontWeight: FontWeight.w500),
                              const Spacer(),
                              AppText(
                                  isBody: true,
                                  text: "${data3.itemName}",
                                  textAlign: TextAlign.start,
                                  fontSize: 75,
                                  color: AppColors.warning,
                                  fontStyle: FontStyle.normal,
                                  fontWeight: FontWeight.bold),
                            ],
                          ),
                          Row(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            mainAxisAlignment: MainAxisAlignment.start,
                            children: [
                              const AppText(
                                  isBody: false,
                                  text: "Item Model No:",
                                  textAlign: TextAlign.start,
                                  fontSize: 75,
                                  color: AppColors.black,
                                  fontStyle: FontStyle.normal,
                                  fontWeight: FontWeight.w500),
                              const Spacer(),
                              AppText(
                                  isBody: true,
                                  text: "${data3.itemModelNumber}",
                                  textAlign: TextAlign.start,
                                  fontSize: 75,
                                  color: AppColors.warning,
                                  fontStyle: FontStyle.normal,
                                  fontWeight: FontWeight.bold),
                            ],
                          ),
                          Row(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            mainAxisAlignment: MainAxisAlignment.start,
                            children: [
                              const AppText(
                                  isBody: false,
                                  text: "Item Quantity:",
                                  textAlign: TextAlign.start,
                                  fontSize: 75,
                                  color: AppColors.black,
                                  fontStyle: FontStyle.normal,
                                  fontWeight: FontWeight.w500),
                              const Spacer(),
                              AppText(
                                  isBody: true,
                                  text: "${data3.itemQuantity}",
                                  textAlign: TextAlign.start,
                                  fontSize: 75,
                                  color: AppColors.warning,
                                  fontStyle: FontStyle.normal,
                                  fontWeight: FontWeight.bold),
                            ],
                          ),
                          Row(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            mainAxisAlignment: MainAxisAlignment.start,
                            children: [
                              const AppText(
                                  isBody: false,
                                  text: "Item Weight:",
                                  textAlign: TextAlign.start,
                                  fontSize: 75,
                                  color: AppColors.black,
                                  fontStyle: FontStyle.normal,
                                  fontWeight: FontWeight.w500),
                              const Spacer(),
                              AppText(
                                  isBody: true,
                                  text: "${data3.itemWeight}Kg",
                                  textAlign: TextAlign.start,
                                  fontSize: 75,
                                  color: AppColors.warning,
                                  fontStyle: FontStyle.normal,
                                  fontWeight: FontWeight.bold),
                            ],
                          ),
                          Row(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            mainAxisAlignment: MainAxisAlignment.start,
                            children: [
                              const AppText(
                                  isBody: false,
                                  text: "Item Cost:",
                                  textAlign: TextAlign.start,
                                  fontSize: 75,
                                  color: AppColors.black,
                                  fontStyle: FontStyle.normal,
                                  fontWeight: FontWeight.w500),
                              const Spacer(),
                              AppText(
                                  isBody: true,
                                  text: moneyFormat(
                                      context, data3.itemCost!.toDouble()),
                                  textAlign: TextAlign.start,
                                  fontSize: 75,
                                  color: AppColors.warning,
                                  fontStyle: FontStyle.normal,
                                  fontWeight: FontWeight.bold),
                            ],
                          ),
                          const Divider(
                            color: AppColors.primaryDark,
                            thickness: 0.3,
                          ),
                          AppText(
                              isBody: true,
                              text: "Origin Details",
                              textAlign: TextAlign.start,
                              fontSize: 65,
                              color: AppColors.black,
                              fontStyle: FontStyle.normal,
                              maxLines: 1,
                              fontWeight: FontWeight.bold),
                          AppText(
                              isBody: true,
                              text: "${data3.senderAddress}",
                              textAlign: TextAlign.start,
                              fontSize: 65,
                              color: AppColors.warning,
                              fontStyle: FontStyle.normal,
                              maxLines: 1,
                              fontWeight: FontWeight.bold),
                          AppText(
                              isBody: true,
                              text: "${data3.senderPhoneNo}",
                              textAlign: TextAlign.start,
                              fontSize: 65,
                              color: AppColors.warning,
                              fontStyle: FontStyle.normal,
                              maxLines: 1,
                              fontWeight: FontWeight.bold),
                          addVerticalSpacing(context, 5),
                          AppText(
                              isBody: true,
                              text: "Destination Details",
                              textAlign: TextAlign.start,
                              fontSize: 75,
                              color: AppColors.black,
                              fontStyle: FontStyle.normal,
                              maxLines: 1,
                              fontWeight: FontWeight.bold),
                          AppText(
                              isBody: true,
                              text: "${data3.recieverAddress}",
                              textAlign: TextAlign.start,
                              fontSize: 65,
                              color: AppColors.warning,
                              fontStyle: FontStyle.normal,
                              maxLines: 1,
                              fontWeight: FontWeight.bold),
                          AppText(
                              isBody: true,
                              text: "${data3.recieverPhoneNo}",
                              textAlign: TextAlign.start,
                              fontSize: 65,
                              color: AppColors.warning,
                              fontStyle: FontStyle.normal,
                              maxLines: 1,
                              fontWeight: FontWeight.bold),
                          addVerticalSpacing(context, 5),
                          AppText(
                              isBody: true,
                              text: "Company Details",
                              textAlign: TextAlign.start,
                              fontSize: 75,
                              color: AppColors.black,
                              fontStyle: FontStyle.normal,
                              maxLines: 1,
                              fontWeight: FontWeight.bold),
                          Column(
                            mainAxisAlignment: MainAxisAlignment.start,
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              AppText(
                                  isBody: true,
                                  text: "Company Name:",
                                  textAlign: TextAlign.start,
                                  fontSize: 65,
                                  color: AppColors.black,
                                  fontStyle: FontStyle.normal,
                                  maxLines: 1,
                                  fontWeight: FontWeight.w500),
                              AppText(
                                  isBody: true,
                                  text: "${data3.companyName}",
                                  textAlign: TextAlign.start,
                                  fontSize: 65,
                                  color: AppColors.warning,
                                  fontStyle: FontStyle.normal,
                                  fontWeight: FontWeight.w800),
                            ],
                          ),
                          addVerticalSpacing(context, 1.2),
                          Column(
                            mainAxisAlignment: MainAxisAlignment.start,
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              AppText(
                                  isBody: true,
                                  text: "Company Address:",
                                  textAlign: TextAlign.start,
                                  fontSize: 65,
                                  color: AppColors.black,
                                  fontStyle: FontStyle.normal,
                                  maxLines: 1,
                                  fontWeight: FontWeight.w500),
                              AppText(
                                  isBody: true,
                                  text: "${data3.companyAddress}",
                                  textAlign: TextAlign.start,
                                  fontSize: 65,
                                  color: AppColors.warning,
                                  fontStyle: FontStyle.normal,
                                  maxLines: 1,
                                  fontWeight: FontWeight.w800),
                            ],
                          ),
                          addVerticalSpacing(context, 1.2),
                          Row(
                            children: [
                              AppText(
                                  isBody: true,
                                  text: "Company Contact",
                                  textAlign: TextAlign.start,
                                  fontSize: 65,
                                  color: AppColors.black,
                                  fontStyle: FontStyle.normal,
                                  maxLines: 1,
                                  fontWeight: FontWeight.w500),
                              Spacer(),
                              AppText(
                                  isBody: true,
                                  text: "${data3.companyPhoneNo}",
                                  textAlign: TextAlign.start,
                                  fontSize: 65,
                                  color: AppColors.warning,
                                  fontStyle: FontStyle.normal,
                                  maxLines: 1,
                                  fontWeight: FontWeight.w800),
                            ],
                          ),
                          Divider(),
                          addVerticalSpacing(context, 5),
                          AppText(
                              isBody: true,
                              text: "Charges",
                              textAlign: TextAlign.start,
                              fontSize: 75,
                              color: AppColors.black,
                              fontStyle: FontStyle.normal,
                              maxLines: 1,
                              fontWeight: FontWeight.bold),
                          Row(
                            children: [
                              AppText(
                                  isBody: true,
                                  text: "Delivery Charges",
                                  textAlign: TextAlign.start,
                                  fontSize: 65,
                                  color: AppColors.black,
                                  fontStyle: FontStyle.normal,
                                  maxLines: 1,
                                  fontWeight: FontWeight.w500),
                              Spacer(),
                              AppText(
                                  isBody: true,
                                  text: moneyFormat(context,
                                      (data3.shippingCost!).toDouble()),
                                  textAlign: TextAlign.start,
                                  fontSize: 65,
                                  color: AppColors.warning,
                                  fontStyle: FontStyle.normal,
                                  maxLines: 1,
                                  fontWeight: FontWeight.w800),
                            ],
                          ),
                          Row(
                            children: [
                              AppText(
                                  isBody: true,
                                  text: "Vat/Tax Services",
                                  textAlign: TextAlign.start,
                                  fontSize: 65,
                                  color: AppColors.black,
                                  fontStyle: FontStyle.normal,
                                  maxLines: 1,
                                  fontWeight: FontWeight.w500),
                              Spacer(),
                              AppText(
                                  isBody: true,
                                  text: moneyFormat(
                                      context, (data3.vatCost!).toDouble()),
                                  textAlign: TextAlign.start,
                                  fontSize: 65,
                                  color: AppColors.warning,
                                  fontStyle: FontStyle.normal,
                                  maxLines: 1,
                                  fontWeight: FontWeight.w800),
                            ],
                          ),
                          Divider(),
                          Row(
                            children: [
                              AppText(
                                  isBody: true,
                                  text: "Total Cost",
                                  textAlign: TextAlign.start,
                                  fontSize: 65,
                                  color: AppColors.black,
                                  fontStyle: FontStyle.normal,
                                  maxLines: 1,
                                  fontWeight: FontWeight.w400),
                              Spacer(),
                              AppText(
                                  isBody: true,
                                  text: moneyFormat(
                                      context,
                                      (data3.shippingCost! + data3.vatCost!)
                                          .toDouble()),
                                  textAlign: TextAlign.start,
                                  fontSize: 65,
                                  color: AppColors.warning,
                                  fontStyle: FontStyle.normal,
                                  maxLines: 1,
                                  fontWeight: FontWeight.w800),
                            ],
                          ),
                          addVerticalSpacing(context, 5),
                          AppText(
                              isBody: true,
                              text: date,
                              textAlign: TextAlign.start,
                              fontSize: 75,
                              color: AppColors.green,
                              fontStyle: FontStyle.normal,
                              fontWeight: FontWeight.w400),
                          const Divider(
                            color: AppColors.primaryDark,
                            thickness: 0.3,
                          ),
                          Align(
                            alignment: Alignment.bottomRight,
                            child: appButton(
                                "Track Item", getScreenWidth(context) / 2,
                                () async {
                              navigateToRoute(
                                  context,
                                  OrderLiveTrackingPage(
                                    orderId: widget.order.id.toString(),
                                    sourceLatitude:
                                        widget.order.senderLatitude!.toDouble(),
                                    sourceLongitude: widget
                                        .order.senderLongitude!
                                        .toDouble(),
                                    destinationLatitude: widget
                                        .order.recieverLatitude!
                                        .toDouble(),
                                    destinationLongitude: widget
                                        .order.recieverLongitude!
                                        .toDouble(),
                                  ));
                            }, AppColors.primary, false),
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
    );
  }
}
