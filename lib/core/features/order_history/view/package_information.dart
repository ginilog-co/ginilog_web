// ignore_for_file: use_build_context_synchronously

import 'package:ginilog_customer_app/core/components/helpers/endpoints.dart';
import 'package:ginilog_customer_app/core/components/helpers/globals.dart';
import 'package:ginilog_customer_app/core/components/utils/app_buttons.dart';
import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/constants.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/money_formatter.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';
import 'package:ginilog_customer_app/core/components/widgets/custom_snackbar.dart';
import 'package:ginilog_customer_app/core/features/home/widget/package_info_page.dart.dart';
import 'package:ginilog_customer_app/core/features/order_history/model/package_orders_model.dart';
import 'package:ginilog_customer_app/core/features/order_history/services/package_order_service.dart';

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
  String generateTransactionReference() {
    return DateTime.now().millisecondsSinceEpoch.toString();
  }

  Future<void> handleFlutterWavePayment() async {
    final Customer customer = Customer(
      name: "${globals.userName}",
      phoneNumber: '+234${widget.userPhone}',
      email: globals.userEmail ?? "guest@example.com",
    );

    final Flutterwave flutterwave = Flutterwave(
      context: context,
      publicKey: Endpoints.flutterWaveTestKey,
      currency: "NGN",
      txRef: generateTransactionReference(),
      amount: (widget.order.shippingCost! + widget.order.vatCost!).toString(),
      redirectUrl: 'https://www.bringmygas.com/',
      customer: customer,
      paymentOptions: "Bank transfer",
      customization: Customization(title: "Gas Payment"),
      isTestMode: false,
    );

    final ChargeResponse response = await flutterwave.charge();
    printData("Payment Response", response);
    if (response.status == "successful") {
      setState(() {
        isLoading = true;
      });
      final response2 = await PackageOrderService().makePayment(
        orderId: widget.order.id.toString(),
        trnxReference: response.txRef!,
        paymentStatus: true,
        paymentChannel: "Flutterwave",
      );
      if (response2.statusCode == 200 || response2.statusCode == 201) {
// await sendNotifications(
        //     "You have successfully placed an order of ${widget}Kg gas.");
        setState(() {
          isLoading = false;
        });
        navigateToRoute(
            context,
            PaymentConfirmationPage(
                totalAmount: widget.order.shippingCost.toString(),
                cardNumber: "card Number"));
        setState(() {
          isLoading = false;
        });
      } else {
        setState(() {
          isLoading = false;
        });
        showCustomSnackbar(context,
            title: "Order Error",
            content: "Order could not Completed because${response2.body}",
            type: SnackbarType.error,
            isTopPosition: false);
      }
    } else {
      setState(() {
        isLoading = false;
      });
      showCustomSnackbar(context,
          title: "Payment Error",
          content: "The payment could not be process",
          type: SnackbarType.error,
          isTopPosition: false);
    }
  }

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
                  backText: 'Send a package',
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
              data3.shippingCost == 0
                  ? Align(
                      alignment: Alignment.bottomRight,
                      child: appButton(
                          "Waiting for the company to update the shipping cost",
                          getScreenWidth(context) / 2,
                          () async {},
                          AppColors.grey2,
                          false),
                    )
                  : Align(
                      alignment: Alignment.bottomRight,
                      child:
                          appButton("Make Payment", getScreenWidth(context) / 2,
                              () async {
                        handleFlutterWavePayment();
                      }, AppColors.primary, isLoading),
                    ),
            ],
          ),
        ),
      ),
    );
  }
}
