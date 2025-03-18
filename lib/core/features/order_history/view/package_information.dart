// ignore_for_file: use_build_context_synchronously

import 'package:ginilog_customer_app/core/components/utils/app_buttons.dart';
import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/money_formatter.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';
import 'package:ginilog_customer_app/core/features/order_history/model/package_orders_model.dart';
import 'package:ginilog_customer_app/core/features/order_history/widget/payment_set.dart';

class PackageInformationPage extends ConsumerStatefulWidget {
  final PackageOrderResponseModel order;
  final String userPhone;
  const PackageInformationPage(
      {super.key, required this.order, required this.userPhone});

  @override
  ConsumerState<PackageInformationPage> createState() =>
      _PackageInformationPageState();
}

class _PackageInformationPageState
    extends ConsumerState<PackageInformationPage> {
  bool isLoading = false;
  int selected = 0;
  String paymentMethodUse = "";
  @override
  Widget build(BuildContext context) {
    final data3 = widget.order;
    return Scaffold(
      backgroundColor: AppColors.white,
      appBar: PreferredSize(
          preferredSize: Size.fromHeight(SizeConfig.heightAdjusted(16)),
          child: Padding(
            padding: EdgeInsets.only(top: SizeConfig.heightAdjusted(10)),
            child: Column(
              children: [
                GlobalBackButton(
                  backText: 'Package Payment',
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
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisAlignment: MainAxisAlignment.start,
            children: [
              const AppText(
                  isBody: true,
                  text: "Package Information",
                  textAlign: TextAlign.start,
                  fontSize: 75,
                  color: AppColors.primary,
                  fontStyle: FontStyle.normal,
                  maxLines: 1,
                  fontWeight: FontWeight.bold),
              Divider(),
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
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  maxLines: 1,
                  fontWeight: FontWeight.w400),
              AppText(
                  isBody: true,
                  text: "${data3.senderPhoneNo}",
                  textAlign: TextAlign.start,
                  fontSize: 65,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  maxLines: 1,
                  fontWeight: FontWeight.w400),
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
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  maxLines: 1,
                  fontWeight: FontWeight.w400),
              AppText(
                  isBody: true,
                  text: "${data3.recieverPhoneNo}",
                  textAlign: TextAlign.start,
                  fontSize: 65,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  maxLines: 1,
                  fontWeight: FontWeight.w400),
              addVerticalSpacing(context, 5),
              AppText(
                  isBody: true,
                  text: "Order Details",
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
                      text: "Item Name",
                      textAlign: TextAlign.start,
                      fontSize: 65,
                      color: AppColors.black,
                      fontStyle: FontStyle.normal,
                      maxLines: 1,
                      fontWeight: FontWeight.w400),
                  Spacer(),
                  AppText(
                      isBody: true,
                      text: "${data3.itemName}",
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
                      text: "Tracking Number",
                      textAlign: TextAlign.start,
                      fontSize: 65,
                      color: AppColors.black,
                      fontStyle: FontStyle.normal,
                      maxLines: 1,
                      fontWeight: FontWeight.w400),
                  Spacer(),
                  AppText(
                      isBody: true,
                      text: "${data3.trackingNum}",
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
                      text: "Item Quantity",
                      textAlign: TextAlign.start,
                      fontSize: 65,
                      color: AppColors.black,
                      fontStyle: FontStyle.normal,
                      maxLines: 1,
                      fontWeight: FontWeight.w400),
                  Spacer(),
                  AppText(
                      isBody: true,
                      text: "${data3.itemQuantity}",
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
                  text: "Company Details",
                  textAlign: TextAlign.start,
                  fontSize: 75,
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
                      fontSize: 65,
                      color: AppColors.black,
                      fontStyle: FontStyle.normal,
                      maxLines: 1,
                      fontWeight: FontWeight.w400),
                  Spacer(),
                  Expanded(
                    child: AppText(
                        isBody: true,
                        text: "${data3.companyName}",
                        textAlign: TextAlign.start,
                        fontSize: 65,
                        color: AppColors.warning,
                        fontStyle: FontStyle.normal,
                        maxLines: 1,
                        fontWeight: FontWeight.w800),
                  ),
                ],
              ),
              Row(
                children: [
                  AppText(
                      isBody: true,
                      text: "Company Address",
                      textAlign: TextAlign.start,
                      fontSize: 65,
                      color: AppColors.black,
                      fontStyle: FontStyle.normal,
                      maxLines: 1,
                      fontWeight: FontWeight.w400),
                  Spacer(),
                  Expanded(
                    child: AppText(
                        isBody: true,
                        text: "${data3.companyAddress}",
                        textAlign: TextAlign.start,
                        fontSize: 65,
                        color: AppColors.warning,
                        fontStyle: FontStyle.normal,
                        maxLines: 1,
                        fontWeight: FontWeight.w800),
                  ),
                ],
              ),
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
                      fontWeight: FontWeight.w400),
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
                      fontWeight: FontWeight.w400),
                  Spacer(),
                  AppText(
                      isBody: true,
                      text: moneyFormat(
                          context, (data3.shippingCost!).toDouble()),
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
                      fontWeight: FontWeight.w400),
                  Spacer(),
                  AppText(
                      isBody: true,
                      text: moneyFormat(context, (data3.vatCost!).toDouble()),
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
                      text: moneyFormat(context,
                          (data3.shippingCost! + data3.vatCost!).toDouble()),
                      textAlign: TextAlign.start,
                      fontSize: 65,
                      color: AppColors.warning,
                      fontStyle: FontStyle.normal,
                      maxLines: 1,
                      fontWeight: FontWeight.w800),
                ],
              ),
              addVerticalSpacing(context, 10),
              AppText(
                  isBody: false,
                  text: data3.shippingCost == 0
                      ? "Waiting for the company to update the shipping cost once the pick up the package"
                      : "",
                  textAlign: TextAlign.start,
                  fontSize: 75,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w500),
              data3.shippingCost == 0
                  ? Align(
                      alignment: Alignment.bottomRight,
                      child: appButton(
                          "Waiting for the company to update the shipping cost",
                          getScreenWidth(context) / 2, () async {
                        //  showPaymentMethodSelection(context);
                      }, AppColors.grey2, false),
                    )
                  : Align(
                      alignment: Alignment.bottomRight,
                      child:
                          appButton("Make Payment", getScreenWidth(context) / 2,
                              () async {
                        showModalBottomSheet(
                          context: context,
                          isScrollControlled: true,
                          shape: RoundedRectangleBorder(
                            borderRadius:
                                BorderRadius.vertical(top: Radius.circular(20)),
                          ),
                          builder: (context) => PaymentMethodBottomSheet(
                            order: widget.order,
                          ),
                        );
                      }, AppColors.primary, isLoading),
                    ),
            ],
          ),
        ),
      ),
    );
  }
}
