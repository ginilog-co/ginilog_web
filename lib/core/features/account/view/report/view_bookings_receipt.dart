import 'dart:convert';

import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/money_formatter.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/customer_book_response_model.dart';

class ViewCustomerBookReceiptPage extends ConsumerStatefulWidget {
  final CustomerBookResponseModel order;
  const ViewCustomerBookReceiptPage({super.key, required this.order});

  @override
  ConsumerState<ViewCustomerBookReceiptPage> createState() =>
      _ViewCustomerBookReceiptPageState();
}

class _ViewCustomerBookReceiptPageState
    extends ConsumerState<ViewCustomerBookReceiptPage> {
  bool isLoading = false;
  String generateTransactionReference() {
    return DateTime.now().millisecondsSinceEpoch.toString();
  }

  @override
  Widget build(BuildContext context) {
    final data3 = widget.order;
    return Scaffold(
      backgroundColor: AppColors.white,
      appBar: PreferredSize(
          preferredSize: Size.fromHeight(SizeConfig.heightAdjusted(12)),
          child: Padding(
            padding: EdgeInsets.only(top: SizeConfig.heightAdjusted(10)),
            child: GlobalBackButton(
              backText: "Booking Receipt",
              showBackButton: true,
              buttonElements: [],
            ),
          )),
      body: SingleChildScrollView(
        child: Padding(
          padding: const EdgeInsets.only(left: 15.0, right: 15.0, top: 0.0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisAlignment: MainAxisAlignment.start,
            children: [
              const AppText(
                  isBody: true,
                  text: "Package Information",
                  textAlign: TextAlign.start,
                  fontSize: 45,
                  color: AppColors.primary,
                  fontStyle: FontStyle.normal,
                  maxLines: 1,
                  fontWeight: FontWeight.bold),
              Divider(),
              AppText(
                  isBody: true,
                  text: "Origin Details",
                  textAlign: TextAlign.start,
                  fontSize: 45,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  maxLines: 1,
                  fontWeight: FontWeight.bold),
              AppText(
                  isBody: true,
                  text: "${data3.customerName}",
                  textAlign: TextAlign.start,
                  fontSize: 35,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  maxLines: 1,
                  fontWeight: FontWeight.w400),
              AppText(
                  isBody: true,
                  text: "${data3.customerPhoneNumber}",
                  textAlign: TextAlign.start,
                  fontSize: 35,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  maxLines: 1,
                  fontWeight: FontWeight.w400),
              addVerticalSpacing(context, 5),
              AppText(
                  isBody: true,
                  text: "Destination Details",
                  textAlign: TextAlign.start,
                  fontSize: 45,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  maxLines: 1,
                  fontWeight: FontWeight.bold),
              AppText(
                  isBody: true,
                  text: "${data3.numberOfGuests}",
                  textAlign: TextAlign.start,
                  fontSize: 35,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  maxLines: 1,
                  fontWeight: FontWeight.w400),
              AppText(
                  isBody: true,
                  text: "${data3.accomodationType}",
                  textAlign: TextAlign.start,
                  fontSize: 35,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  maxLines: 1,
                  fontWeight: FontWeight.w400),
              addVerticalSpacing(context, 5),
              AppText(
                  isBody: true,
                  text: "Order Details",
                  textAlign: TextAlign.start,
                  fontSize: 45,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  maxLines: 1,
                  fontWeight: FontWeight.bold),
              Row(
                children: [
                  AppText(
                      isBody: true,
                      text: "Item Name",
                      textAlign: TextAlign.start,
                      fontSize: 35,
                      color: AppColors.black,
                      fontStyle: FontStyle.normal,
                      maxLines: 1,
                      fontWeight: FontWeight.w400),
                  Spacer(),
                  AppText(
                      isBody: true,
                      text: "${data3.noOfDays}",
                      textAlign: TextAlign.start,
                      fontSize: 35,
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
                      text: "Tracking Number",
                      textAlign: TextAlign.start,
                      fontSize: 35,
                      color: AppColors.black,
                      fontStyle: FontStyle.normal,
                      maxLines: 1,
                      fontWeight: FontWeight.w400),
                  Spacer(),
                  AppText(
                      isBody: true,
                      text: "${data3.ticketNum}",
                      textAlign: TextAlign.start,
                      fontSize: 35,
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
                      text: "Item Quantity",
                      textAlign: TextAlign.start,
                      fontSize: 35,
                      color: AppColors.black,
                      fontStyle: FontStyle.normal,
                      maxLines: 1,
                      fontWeight: FontWeight.w400),
                  Spacer(),
                  AppText(
                      isBody: true,
                      text: "${data3.paymentChannel}",
                      textAlign: TextAlign.start,
                      fontSize: 35,
                      color: AppColors.warning,
                      fontStyle: FontStyle.normal,
                      maxLines: 1,
                      fontWeight: FontWeight.w800),
                ],
              ),
              addVerticalSpacing(context, 5),
              AppText(
                  isBody: true,
                  text: "Company Details",
                  textAlign: TextAlign.start,
                  fontSize: 45,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  maxLines: 1,
                  fontWeight: FontWeight.bold),
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  AppText(
                      isBody: true,
                      text: "Company Name",
                      textAlign: TextAlign.start,
                      fontSize: 35,
                      color: AppColors.black,
                      fontStyle: FontStyle.normal,
                      maxLines: 1,
                      fontWeight: FontWeight.w400),
                  Spacer(),
                  Expanded(
                    child: AppText(
                        isBody: true,
                        text: "${data3.trnxReference}",
                        textAlign: TextAlign.start,
                        fontSize: 35,
                        color: AppColors.warning,
                        fontStyle: FontStyle.normal,
                        maxLines: 1,
                        fontWeight: FontWeight.w800),
                  ),
                ],
              ),
              Divider(),
              addVerticalSpacing(context, 5),
              AppText(
                  isBody: true,
                  text: "Charges",
                  textAlign: TextAlign.start,
                  fontSize: 45,
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
                      fontSize: 35,
                      color: AppColors.black,
                      fontStyle: FontStyle.normal,
                      maxLines: 1,
                      fontWeight: FontWeight.w400),
                  Spacer(),
                  AppText(
                      isBody: true,
                      text: moneyFormat(context, (data3.totalCost!).toDouble()),
                      textAlign: TextAlign.start,
                      fontSize: 35,
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
                      fontSize: 35,
                      color: AppColors.black,
                      fontStyle: FontStyle.normal,
                      maxLines: 1,
                      fontWeight: FontWeight.w400),
                  Spacer(),
                  AppText(
                      isBody: true,
                      text: moneyFormat(context, (data3.totalCost!).toDouble()),
                      textAlign: TextAlign.start,
                      fontSize: 35,
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
                      fontSize: 35,
                      color: AppColors.black,
                      fontStyle: FontStyle.normal,
                      maxLines: 1,
                      fontWeight: FontWeight.w400),
                  Spacer(),
                  AppText(
                      isBody: true,
                      text: moneyFormat(context,
                          (data3.totalCost! + data3.totalCost!).toDouble()),
                      textAlign: TextAlign.start,
                      fontSize: 35,
                      color: AppColors.warning,
                      fontStyle: FontStyle.normal,
                      maxLines: 1,
                      fontWeight: FontWeight.w800),
                ],
              ),
              addVerticalSpacing(context, 1),
              Image.memory(
                base64Decode(data3.qrCode!),
                height: SizeConfig.heightAdjusted(50),
                width: SizeConfig.widthAdjusted(200),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
